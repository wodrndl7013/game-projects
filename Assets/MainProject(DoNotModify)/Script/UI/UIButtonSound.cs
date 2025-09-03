using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public string hoverSound; // Hover 사운드 키
    public string clickSound; // Click 사운드 키

    // 마우스 커서가 올라갔을 때 실행
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(hoverSound))
        {
            SoundManager.Instance.PlaySound(hoverSound);
        }
    }

    // 클릭했을 때 실행
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(clickSound))
        {
            SoundManager.Instance.PlaySound(clickSound);
        }
    }
}