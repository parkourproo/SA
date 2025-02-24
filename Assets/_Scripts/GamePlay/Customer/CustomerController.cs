using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomerController : MonoBehaviour
{
    private GridManager gridManager;
    public BusController busController;
    private int doorRow, doorCol;
    public Button pauseButton;

    public static int totalCus; //khi khách ngồi vào ghế mới cập nhật
    
    private Dictionary<string, int> checkMoveSeat = new();
    public Dictionary<string, bool> listSeatCusMovingIn = new();

    private CustomerMovementSubject movementSubject;
    public MoveSeat moveSeat;

    public ParticleSystem confetti;
    IEnumerator Start()
    {
        gridManager = GridManager.Instance;
        yield return new WaitUntil(() => gridManager.doneGrid != "");
        yield return new WaitUntil(() => CustomerManager.doneCustomer != "");
        totalCus = CustomerManager.Instance.totalCustomer;
        doorRow = 0;
        doorCol = gridManager.GetGridCol() - 1;

        movementSubject = CustomerMovementSubject.Instance;
    }



    public IEnumerator CustomerGoToSeat()
    {
        while (true)
        {
            Customer customer;
            if (CustomerManager.Instance.customerList.Count != 0)
            {
                customer = CustomerManager.Instance.customerList.Peek();
            }
            else break;
            Stack<GridCell> path = BFS_FindPath.Instance.BFS(gridManager.gridCells[doorRow,
                doorCol], customer.customerColor);

            if (path == null)
            {
                break;
            }
            else if (path.Count > 0)
            {
                if (CustomerManager.Instance.customerList.Count == 1)
                {
                    pauseButton.enabled = false;
                    TimeController.stopTimeDelegate?.Invoke();
                }
                Seat foundSeat = BFS_FindPath.Instance.GetFoundSeat();
                CustomerManager.Instance.customerList.Dequeue();

                string seatName = foundSeat.associatedObject.name;
                if(listSeatCusMovingIn.ContainsKey(seatName))
                {
                    listSeatCusMovingIn[seatName] = true;
                }
                else
                {
                    listSeatCusMovingIn.Add(seatName, true);
                }
                // mỗi khách đi vào, sẽ tăng giá trị check lên 1, để kiểm tra khi
                // khách cuối cùng đi vào ghế thì mới cho ghế đó di chuyển được, ví
                // dụ: ghế đôi có 2 khách đi vào trong cùng lần drop seat, khi khách
                // thứ 2 đi vào thì mới cho ghế đó di chuyển được
                if (checkMoveSeat.ContainsKey(seatName))
                {
                    checkMoveSeat[seatName]++;
                }
                else
                {
                    //int startCus = 1;
                    checkMoveSeat.Add(seatName, 1);
                }
                foundSeat.movable = false;
                StartCoroutine(MoveToSeat(customer, path, foundSeat, checkMoveSeat[seatName]));
            }
        }
        CustomerManager.Instance.LineUpCustomer();
        yield return null;

    }

    private IEnumerator MoveToSeat(Customer customer, Stack<GridCell> path, Seat foundSeat, int key)
    {
        // Thông báo bắt đầu di chuyển
        movementSubject.IncrementMovingCustomers();

        yield return StartCoroutine(CustomerMovement.Move(
                customer.associatedObject, CustomerManager.Instance.GetFirstInLinePosition()));

        while (path.Count > 1)
        {

            GridCell cell = path.Pop();
            yield return StartCoroutine(CustomerMovement.Move(
                customer.associatedObject, cell.tile.transform.position
                + gridManager.GetThickTilePrefab() / 2));
        }

        // The final position where the customer sits
        GridCell seatPosition = path.Pop();

        yield return StartCoroutine(CustomerMovement.Sit(
                customer.associatedObject, seatPosition.tile.transform.position
                + gridManager.GetThickTilePrefab() / 2, seatPosition));

        // Thông báo kết thúc di chuyển
        movementSubject.DecrementMovingCustomers();

        totalCus--;
        //Debug.Log(totalCus);
        string seatName = foundSeat.associatedObject.name;
        if (checkMoveSeat[seatName] == key)
        {
            foundSeat.movable = foundSeat.seatColor != EnumColor.Grey;
            listSeatCusMovingIn[seatName] = false;
            //Debug.Log("Seat: " + seatName + " is movable");
        }
        else
        {
            foundSeat.movable = false;
        }
        if(totalCus == 0)
        {
            StartCoroutine(Win());
        }
    }


    public IEnumerator Win()
    {
        if (moveSeat.IsDragging())
        {
            moveSeat.DropSeat();
        }
        pauseButton.enabled = false;
        MoveSeat.setSelectionEnable?.Invoke(false);
        confetti.Play();
        SoundPlayer.Instance.PlaySoundConfetti();
        SoundPlayer.Instance.PlaySoundWin();
        yield return StartCoroutine(busController.CloseDoor());
        CanvasController.onWinDo?.Invoke();
        int unlockLevel = GridManager.Instance.GetLevel();
        PlayerPrefs.SetInt("UnlockLevel", unlockLevel + 1);
        SaveSystem.SaveHeart(SaveSystem.GetHeart() + 1);
        //Debug.Log("Add To: " + SaveSystem.GetHeart());

    }

    
}
