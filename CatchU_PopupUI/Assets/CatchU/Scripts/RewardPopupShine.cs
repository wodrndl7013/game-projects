using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardPopupShine : MonoBehaviour
{
    [SerializeField] RectTransform maskArea; // 마스크 영역
    [SerializeField] RectTransform shineBar; // 하이라이트 바
    [SerializeField] float duration = 0.25f;
    [SerializeField] float delay = 0.05f;

    public void PlayOnce()
    {
        gameObject.SetActive(true);
        StartCoroutine(Co());
    }

    IEnumerator Co()
    {
        yield return new WaitForSecondsRealtime(delay);
        float w = maskArea.rect.width;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duration);
            float x = Mathf.Lerp(-w * 0.5f, w * 0.5f, p);
            var pos = shineBar.anchoredPosition;
            shineBar.anchoredPosition = new Vector2(x, pos.y);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}

