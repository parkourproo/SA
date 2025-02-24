using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreezeTimeImage : MonoBehaviour
{
    public Image freezeTimeImage;
    void Start()
    {
        freezeTimeImage.color = new Color(255, 255, 255, 0);
    }

    //public IEnumerator AnimateFreeze(float freezeDuration)
    //{
    //    // Nếu freezeDuration nhỏ hơn 2 thì đảm bảo không âm cho interval
    //    float holdTime = Mathf.Max(freezeDuration - 2f, 0f);

    //    Sequence sequence = DOTween.Sequence();
    //    sequence.Append(freezeTimeImage.DOFade(1f, 1f));       // Fade in trong 1 giây
    //    sequence.AppendInterval(holdTime);                    // Giữ nguyên alpha = 1
    //    sequence.Append(freezeTimeImage.DOFade(0f, 1f));       // Fade out trong 1 giây
    //    yield return sequence.WaitForCompletion();
    //}

    public IEnumerator AnimateFreeze(float freezeDuration)
    {
        float holdTime = Mathf.Max(freezeDuration - 2f, 0f);

        yield return FadeImage(0f, 1f, 1f);

        // Thay thế WaitForSeconds bằng cách đếm thời gian thủ công
        yield return WaitForSecondsWithPause(holdTime);

        yield return FadeImage(1f, 0f, 1f);
    }

    private IEnumerator WaitForSecondsWithPause(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Nếu game bị tạm dừng, không tăng elapsedTime
            if (!TimeController.Instance.isPausePanelActive)
            {
                elapsedTime += Time.deltaTime;
            }
            yield return null; // Chờ đến khung hình tiếp theo
        }
    }

    private IEnumerator FadeImage(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color color = freezeTimeImage.color;

        while (elapsedTime < duration)
        {
            // Kiểm tra nếu bảng tạm dừng đang hoạt động
            while (TimeController.Instance.isPausePanelActive)
            {
                yield return null; // Chờ đến khung hình tiếp theo
            }

            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            freezeTimeImage.color = color;
            yield return null;
        }

        // Đảm bảo giá trị cuối cùng chính xác
        color.a = endAlpha;
        freezeTimeImage.color = color;
    }

}
