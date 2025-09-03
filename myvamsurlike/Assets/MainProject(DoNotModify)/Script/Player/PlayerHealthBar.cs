using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Image healthFillImage; // 체력바 채워지는 이미지
    private float originalWidth;  // 체력바의 원래 너비
    private RectTransform healthFillRect; // 체력바 RectTransform 참조

    private void Start()
    {
        healthFillRect = healthFillImage.rectTransform;
        originalWidth = healthFillRect.sizeDelta.x; // 체력바의 초기 너비 저장
    }

    // 최대 체력에 맞춰 초기화
    public void SetMaxHealth(float maxHealth)
    {
        healthFillRect.sizeDelta = new Vector2(originalWidth, healthFillRect.sizeDelta.y);
        healthFillRect.anchoredPosition = new Vector2(0, healthFillRect.anchoredPosition.y); // 오른쪽 정렬
    }

    // 체력에 따라 체력바 업데이트
    public void SetHealth(float currentHealth, float maxHealth)
    {
        float healthPercent = currentHealth / maxHealth;
        float newWidth = originalWidth * healthPercent;

        // 체력바의 크기 및 위치 조정
        healthFillRect.sizeDelta = new Vector2(newWidth, healthFillRect.sizeDelta.y);
        healthFillRect.anchoredPosition = new Vector2(-(originalWidth - newWidth) / 2, healthFillRect.anchoredPosition.y);
    }
}