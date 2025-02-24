using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : Singleton<TimeController>, ICustomerMovementObserver
{
    public delegate void MyDelegate();
    public static MyDelegate startTimeDelegate;
    public static MyDelegate stopTimeDelegate;
    public static MyDelegate pauseTimeDelegate;
    public static MyDelegate resumeTimeDelegate;

    private GridManager gridManager;
    public BusController busController;

    private int timeOfLevel;
    public Image green;
    public TextMeshProUGUI timeShow;
    private int remainSecond;
    public static bool hasWon;
    private bool isSetUpTime = false;
    private bool isDraggingSeat = false;
    public bool isPaused = false; // Thêm biến kiểm soát trạng thái pause
    private float startTime;
    private float elapsedTime; // Thời gian đã trôi qua
    private bool isTimeOut = false;
    public Button pauseButton;

    public bool isCustomerMoveInTimeOut = false;
    public bool isPausePanelActive = false;
    private float remainingFreezeTime = 0f;
    public MoveSeat moveSeat;

    public CustomerController customerController;
    IEnumerator Start()
    {
        gridManager = GridManager.Instance;
        yield return new WaitUntil(() => gridManager.doneGrid != "");
        timeOfLevel = gridManager.levelData.timeInSecond;
        isSetUpTime = true;
        hasWon = false;
        CustomerMovementSubject.Instance.RegisterObserver(this);

    }

    private void OnEnable()
    {
        startTimeDelegate += StartGame;
        stopTimeDelegate += StopTime;
        pauseTimeDelegate += PauseTime;
        resumeTimeDelegate += ResumeTime;
    }

    private void OnDisable()
    {
        startTimeDelegate -= StartGame;
        stopTimeDelegate -= StopTime;
        pauseTimeDelegate -= PauseTime;
        resumeTimeDelegate -= ResumeTime;
    }

    void Update()
    {
        if (isSetUpTime && !isPaused) // Chỉ cập nhật thời gian nếu không bị pause
        {
            if (hasWon || isTimeOut) return;

            if (!isDraggingSeat)
            {
                startTime = Time.time;
                remainSecond = timeOfLevel;
            }
            else
            {
                elapsedTime = Time.time - startTime;
                float elapsedRatio = elapsedTime / timeOfLevel;

                if (elapsedRatio < 0.5f)
                {
                    float r = Mathf.Lerp(0, 255, elapsedRatio * 2);
                    green.color = new Color(r / 255f, 1f, 0f);
                }
                else
                {
                    float g = Mathf.Lerp(1f, 0f, (elapsedRatio - 0.5f) * 2);
                    green.color = new Color(1f, g, 0f);
                }

                green.fillAmount = 1 - (elapsedTime / timeOfLevel);
                remainSecond = timeOfLevel - (int)elapsedTime;

                if (remainSecond < 0)
                {
                    remainSecond = 0;
                    isTimeOut = true;
                    StartCoroutine(HandleTimeOut());
                }
            }

            timeShow.text = FormatTime(remainSecond);
        }
    }

    void StartGame()
    {
        isDraggingSeat = true;
        isPaused = false;
    }

    void StopTime()
    {
        hasWon = true;
    }

    public void PauseTime()
    {
        isPaused = true;
    }

    public void ResumeTime()
    {
        if (!FreezeTimeButtonController.isFreezeTime)
        {
            isPaused = false;
            startTime = Time.time - elapsedTime;
        }
    }

    string FormatTime(int seconds)
    {
        int minutes = seconds / 60;
        seconds %= 60;

        string formattedMinutes = minutes < 10 ? "0" + minutes.ToString() : minutes.ToString();
        string formattedSeconds = seconds < 10 ? "0" + seconds.ToString() : seconds.ToString();

        return $"{formattedMinutes}:{formattedSeconds}";
    }

    private IEnumerator HandleTimeOut()
    {
        if (SaveSystem.GetHeart() == HeartManager.maxHeartCount - 1)
        {
            SaveSystem.SaveTime();
            SaveSystem.SaveRemainderSec(HeartManager.secondsToRecoverHeart);
            //Debug.Log("Save time");      
        }
        if (moveSeat.IsDragging())
        {
            moveSeat.DropSeat();
        }
        MoveSeat.setSelectionEnable?.Invoke(false);
        pauseButton.enabled = false;

        yield return new WaitUntil(() => !isCustomerMoveInTimeOut);
        if (CustomerController.totalCus != 0)
        {
            // hết giờ mà còn khách thì thua
            yield return StartCoroutine(busController.CloseDoor());
            SoundPlayer.Instance.PlaySoundLoose();
            CanvasController.onLoseDo?.Invoke();
        }
    }

    //public void AddTime(int additionalSeconds)
    //{
    //    if (!isSetUpTime || hasWon || isTimeOut) return;

    //    if (elapsedTime >= additionalSeconds)
    //    {
    //        startTime += additionalSeconds;
    //    }
    //    else
    //    {
    //        startTime = Time.time;
    //    }
    //}

    public IEnumerator FreezeTime(float freezeDuration)
    {
        FreezeTimeButtonController.isFreezeTime = true;
        PauseTime();
        remainingFreezeTime = freezeDuration;

        while (remainingFreezeTime > 0)
        {
            if (!isPausePanelActive)
            {
                remainingFreezeTime -= Time.deltaTime;
            }
            yield return null;
        }

        FreezeTimeButtonController.isFreezeTime = false;
        ResumeTime();
    }

    public void OnCustomerMovementChanged(bool isMoving)
    {
        if (isMoving)
        {
            isCustomerMoveInTimeOut = true;
        }
        else
        {
            isCustomerMoveInTimeOut = false;

        }
    }
}
