using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FreezeTimeButtonController : MonoBehaviour
{
    //public TimeController timeController;
    public Button freezeTimeButton;
    public TextMeshProUGUI helpCountText;
    public BuyHelpItem buyHelpItem;
    public FreezeTimeImage freezeTimeImage;
    public Image filledImage;

    public static bool isFreezeTime; //ngăn dùng trợ giúp khi đang có hiệu lực
    private int timeFreezeDuration = 10;


    private int helpCount;

    private void Start()
    {
        freezeTimeButton.onClick.AddListener(OnMoreTimeButtonClick);
        helpCount = 1;
        UpdateHelpCountUI();
        isFreezeTime = false;
        filledImage.enabled = false;
    }

    void OnMoreTimeButtonClick()
    {
        // Nếu người chơi chưa kéo ghế nào (thời gian chưa chạy) thì không dùng được
        if (!MoveSeat.isDragFirstSeat)
        {
            return;
        }
        // Nếu đang trong thời gian hiệu lực thì không dùng được
        if (isFreezeTime)
        {
            return;
        }
        if (TimeController.hasWon)
        {
            return;
        }


        if (helpCount <= 0)
        {
            Debug.Log("Hết lượt trợ giúp tăng thời gian");
            buyHelpItem.ShowBuyHelpItemPanel(1);
            return;
        }
        helpCount--;
        filledImage.enabled = true;
        UpdateHelpCountUI();
        StartCoroutine(TimeController.Instance.FreezeTime(timeFreezeDuration));
        StartCoroutine(freezeTimeImage.AnimateFreeze(timeFreezeDuration));
        StartCoroutine(RotateImg());
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

    IEnumerator RotateImg()
    {
        filledImage.fillAmount = 1;
        while (isFreezeTime)
        {
            if (!TimeController.Instance.isPausePanelActive)
            {

                filledImage.fillAmount -= (float)1 / timeFreezeDuration * Time.deltaTime;
            }
            yield return null;
        }
        filledImage.enabled = false;
    }
}