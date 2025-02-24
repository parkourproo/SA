﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro; // Thêm thư viện DOTween

public class PaintSeat : MonoBehaviour
{
    public Button paintSeatButton;
    public TextMeshProUGUI helpCountText;
    public BuyHelpItem buyHelpItem;

    private int helpCount;

    private List<Transform> animatedSeats = new List<Transform>();
    private bool isPainSeatMode = false;

    public CustomerController customerController;
    private void Start()
    {
        paintSeatButton.onClick.AddListener(OnPaintSeatButtonClick);
        helpCount = 1;
        UpdateHelpCountUI(); // Cập nhật UI khi bắt đầu
    }

    private void Update()
    {
        if (isPainSeatMode)
        {
            if (Input.GetMouseButtonDown(0)) // Nhấn chuột hoặc chạm vào màn hình
            {
                paintSeatButton.interactable = true;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    GameObject hitObject = hit.collider.gameObject;
                    if (hitObject.name.Contains("seat"))
                    {
                        string seatName = hitObject.name;
                        if(customerController.listSeatCusMovingIn.ContainsKey(seatName))
                        {
                            if (customerController.listSeatCusMovingIn[seatName] == true)
                            {
                                StopSeatAnimation();
                                isPainSeatMode = false;
                                return;
                            }
                        }

                        Seat seatHit = hitObject.GetComponent<SeatDataa>().seat;
                        if (seatHit.seatColor == EnumColor.Grey)
                        {
                            seatHit.seatColor = EnumColor.Blue;
                            Renderer seatRenderer = hitObject.GetComponentInChildren<Renderer>();
                            seatRenderer.material.color = ColorHelper.GetColor(EnumColor.Blue);
                            seatHit.movable = true;

                            // Giảm số lần trợ giúp
                            helpCount--;
                            UpdateHelpCountUI(); // Cập nhật UI

                            StopSeatAnimation();
                            isPainSeatMode = false;
                        }
                        else
                        {
                            StopSeatAnimation();
                            isPainSeatMode = false;
                        }
                    }
                    else
                    {
                        StopSeatAnimation();
                        isPainSeatMode = false;
                    }
                }
            }
        }
    }

    void OnPaintSeatButtonClick()
    {
        if (TimeController.hasWon)
        {
            return;
        }
        if (helpCount <= 0)
        {
            Debug.Log("Hết lượt trợ giúp sơn ghế");
            buyHelpItem.ShowBuyHelpItemPanel(2);
            return;
        }
        paintSeatButton.interactable = false;
        isPainSeatMode = true;
        LevelData levelData = GridManager.Instance.levelData;
        GridCell[,] gridCells = GridManager.Instance.gridCells;
        animatedSeats.Clear(); // Xóa danh sách cũ

        GameObject allSeats = GameObject.Find("AllSeats");


        foreach (Transform seatData in allSeats.transform)
        {
            SeatDataa seatDataa = seatData.gameObject.GetComponent<SeatDataa>();
            string seatName = seatData.gameObject.name;
            if (customerController.listSeatCusMovingIn.ContainsKey(seatName))
            {
                if (seatDataa.seat.seatColor == EnumColor.Grey && customerController.listSeatCusMovingIn[seatName] == false)
                {
                    animatedSeats.Add(seatData); // Lưu lại để có thể dừng sau này
                    // Tạo hiệu ứng phóng to thu nhỏ lặp lại
                    seatData.DOScale(1.2f, 0.5f) // Phóng to 1.2 lần
                        .SetLoops(-1, LoopType.Yoyo) // Lặp lại vô hạn
                        .SetEase(Ease.InOutSine);
                }
            }
            else
            {
                if (seatDataa.seat.seatColor == EnumColor.Grey)
                {
                    animatedSeats.Add(seatData);
                    seatData.DOScale(1.2f, 0.5f)
                        .SetLoops(-1, LoopType.Yoyo)
                        .SetEase(Ease.InOutSine);
                }
            }
        }
    }

    public void StopSeatAnimation()
    {
        foreach (Transform seat in animatedSeats)
        {
            if (seat != null)
            {
                seat.localScale = Vector3.one; // Đưa về kích thước ban đầu
                seat.DOKill(); // Dừng hiệu ứng DOTween trên ghế
            }
        }
        animatedSeats.Clear(); // Xóa danh sách ghế đã lưu
    }

    private void UpdateHelpCountUI()
    {
        helpCountText.text = helpCount.ToString();
        if (helpCount <= 0)
        {
            helpCountText.text = "+";
        }
    }

    public void AddHelpCount(int count)
    {
        helpCount += count;
        UpdateHelpCountUI();
    }
}
