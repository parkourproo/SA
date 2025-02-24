using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeartManager : MonoBehaviour
{

    public TextMeshProUGUI heartText;
    public TextMeshProUGUI timeText;
    public Button addHeartButton;
    public TextMeshProUGUI numHeartInHeartImage;
    public TextMeshProUGUI nextHeartTime;
    public TextMeshProUGUI priceOfRefill;

    public BuyHeartPanel buyHeartPanel;

    public static int priceOneHeart = 180;
    private int heartCount;
    public static int maxHeartCount = 5;
    public static int secondsToRecoverHeart = 1800;
    private int remainSeconds;
    private float timer = 0f; // Biến đếm thời gian trôi qua

    public static bool isEnterGame = false;
    void Start()
    {
        InitialTime();
        isEnterGame = true;
    }
    void OnApplicationPause(bool pauseStatus)
    {
        // khi vào game hoặc load sang scene này thì chỉ cập nhật Time bởi hàm InitialTime trong start
        if (!pauseStatus && isEnterGame)
        {
            InitialTime();
            //Debug.Log("InitialTime");
        }
    }
    private void Update()
    {
        heartCount = SaveSystem.GetHeart();
        SaveSystem.SaveTime();
        if (heartCount < maxHeartCount)
        {
            // Đếm thời gian trôi qua
            timer += Time.deltaTime;

            // Nếu đã trôi qua 1 giây
            if (timer >= 1f)
            {
                timer = 0f; // Reset timer
                remainSeconds--; // Giảm thời gian còn lại
                // thời gian còn lại <= 0, những gì thay đổi khi thêm mạng thì để đây
                if (remainSeconds <= 0)
                {
                    heartCount++;
                    remainSeconds = secondsToRecoverHeart; // Reset thời gian đếm ngược
                    //SaveSystem.SaveRemainderSec(remainSeconds);
                    // Đảm bảo heartCount không vượt quá maxHeartCount
                    heartCount = Mathf.Min(heartCount, maxHeartCount);
                    numHeartInHeartImage.text = heartCount.ToString();
                    priceOfRefill.text = (priceOneHeart * (maxHeartCount - heartCount)).ToString();
                    heartText.text = heartCount.ToString();
                    SaveSystem.SaveHeart(heartCount);
                }
                SaveSystem.SaveRemainderSec(remainSeconds);

                // Cập nhật timeText, những gì thay đổi thường xuyên thì để đây
                timeText.text = TimeCalculattion.ConvertSecondsToMinutesSeconds(remainSeconds);
                nextHeartTime.text = timeText.text;

            }
        }
        if (heartCount >= maxHeartCount)
        {
            heartText.text = heartCount.ToString();
            addHeartButton.interactable = false;
            timeText.text = "MAX";
            if (buyHeartPanel.buyHeartPanelContainer.activeSelf)
            {
                buyHeartPanel.OnClosePanelButtonClick();
            }
            return;
        }
    }

    void InitialTime()
    {
        heartCount = SaveSystem.GetHeart();
        remainSeconds = SaveSystem.GetRemainderSec();

        heartCount = heartCount == -1 ? maxHeartCount : heartCount;
        if (heartCount < maxHeartCount)
        {
            int timeBetween = TimeCalculattion.CalculateTimeDifferenceInSeconds(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"), SaveSystem.GetTime());
            if (timeBetween >= remainSeconds)
            {
                heartCount++;
                heartCount += (timeBetween - remainSeconds) / secondsToRecoverHeart;
                remainSeconds = secondsToRecoverHeart - (timeBetween - remainSeconds) % secondsToRecoverHeart;
            }
            else
            {
                remainSeconds -= timeBetween;
            }
            //heartCount = 3;
        }
        heartCount = Mathf.Min(heartCount, maxHeartCount);


        numHeartInHeartImage.text = heartCount.ToString();
        priceOfRefill.text = (priceOneHeart * (maxHeartCount - heartCount)).ToString();
        heartText.text = heartCount.ToString();

        SaveSystem.SaveHeart(heartCount);

        // Cập nhật timeText ban đầu
        if (heartCount < maxHeartCount)
        {
            addHeartButton.interactable = true;
            timeText.text = TimeCalculattion.ConvertSecondsToMinutesSeconds(remainSeconds);
        }
        else
        {
            addHeartButton.interactable = false;
            timeText.text = "MAX";
        }
    }

}
