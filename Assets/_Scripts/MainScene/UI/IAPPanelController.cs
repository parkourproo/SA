using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IAPPanelController : MonoBehaviour
{
    public GameObject IAPPanelContainer;
    public GameObject IAPPanel;
    public Button closeButton;
    public Button addCoinButton;

    private Vector3 originalScale;
    void Start()
    {
        IAPPanelContainer.SetActive(false);
        closeButton.onClick.AddListener(OnCloseButtonClick);
        addCoinButton.onClick.AddListener(ShowIAPPanel);
        originalScale = IAPPanel.transform.localScale;
    }

    public void ShowIAPPanel()
    {
        IAPPanelContainer.GetComponent<CanvasGroup>().alpha = 0f;
        IAPPanel.transform.localScale = Vector3.zero;
        IAPPanelContainer.SetActive(true);

        IAPPanelContainer.GetComponent<CanvasGroup>().DOFade(1f, 0.3f).SetUpdate(true);
        IAPPanel.transform.DOScale(originalScale, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    void OnCloseButtonClick()
    {
        HideIAPPanel();
    }

    public void HideIAPPanel()
    {
        IAPPanelContainer.GetComponent<CanvasGroup>().DOFade(0f, 0.5f).SetUpdate(true)
            .OnComplete(() =>
            {
                IAPPanelContainer.SetActive(false);
            });
        IAPPanel.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).SetUpdate(true);
    }
}