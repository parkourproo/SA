using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class CanvasController : MonoBehaviour
{
    public delegate void MyDelegate();

    public static MyDelegate onWinDo;
    public static MyDelegate onLoseDo;
    public GameObject winPanel;
    public GameObject losePanel;
    public Button continueLooseButton;

    private CanvasGroup winPanelCanvasGroup;
    private CanvasGroup losePanelCanvasGroup;
    private Vector3 originalWinPanelScale;
    private Vector3 originalLosePanelScale;
    private float panelAnimationDuration = 0.3f;

    private void OnEnable()
    {
        onWinDo += ShowWinPanel;
        onLoseDo += ShowLosePanel;
    }

    private void OnDisable()
    {
        onWinDo -= ShowWinPanel;
        onLoseDo -= ShowLosePanel;
    }

    void Start()
    {
        winPanel.SetActive(false);
        originalWinPanelScale = winPanel.transform.localScale;
        losePanel.SetActive(false);
        originalLosePanelScale = losePanel.transform.localScale;

        continueLooseButton.onClick.AddListener(OnContinueButtonClick);


        if (winPanel.GetComponent<CanvasGroup>() == null)
        {
            winPanelCanvasGroup = winPanel.AddComponent<CanvasGroup>();
        }
        winPanelCanvasGroup = winPanel.GetComponent<CanvasGroup>();

        if (losePanel.GetComponent<CanvasGroup>() == null)
        {
            losePanelCanvasGroup = losePanel.AddComponent<CanvasGroup>();
        }
        losePanelCanvasGroup = losePanel.GetComponent<CanvasGroup>();
    }



    private void ShowWinPanel()
    {
        PreparePanelForAnimation(winPanel, winPanelCanvasGroup);
        AnimatePanelShow(winPanel, winPanelCanvasGroup, originalWinPanelScale);
    }

    private void ShowLosePanel()
    {
        PreparePanelForAnimation(losePanel, losePanelCanvasGroup);
        AnimatePanelShow(losePanel, losePanelCanvasGroup, originalLosePanelScale);
    }

    void PreparePanelForAnimation(GameObject panel, CanvasGroup canvasGroup)
    {
        panel.SetActive(true);
        canvasGroup.alpha = 0f;
        panel.transform.localScale = Vector3.zero;
    }

    void AnimatePanelShow(GameObject panel, CanvasGroup canvasGroup, Vector3 targetScale)
    {
        canvasGroup.DOFade(1f, panelAnimationDuration).SetUpdate(true);
        panel.transform.DOScale(targetScale, panelAnimationDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    private void OnContinueButtonClick()
    {
        if (winPanel.activeSelf)
        {
            AnimatePanelHide(winPanel, winPanelCanvasGroup, originalWinPanelScale, () =>
            {
                winPanel.SetActive(false);
                LoadMainScene();
            });
        }
        else if (losePanel.activeSelf)
        {
            AnimatePanelHide(losePanel, losePanelCanvasGroup, originalLosePanelScale, () =>
            {
                losePanel.SetActive(false);
                LoadMainScene();
            });
        }
    }

    void AnimatePanelHide(GameObject panel, CanvasGroup canvasGroup, Vector3 originalScale, System.Action onComplete)
    {
        canvasGroup.DOFade(0f, panelAnimationDuration).SetUpdate(true);
        panel.transform.DOScale(Vector3.zero, panelAnimationDuration)
            .SetEase(Ease.InBack)
            .SetUpdate(true)
            .OnComplete(() => onComplete?.Invoke());
    }

    void LoadMainScene()
    {
        HeartManager.isEnterGame = false;
        SceneManager.LoadScene("MainScene");
    }

    void OnDestroy()
    {
        DOTween.KillAll();
    }
}