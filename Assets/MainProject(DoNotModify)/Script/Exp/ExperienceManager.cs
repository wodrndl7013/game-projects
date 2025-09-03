using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceManager : Singleton<ExperienceManager>
{
    public int currentExperience = 0; // 현재 경험치

    public int level = 1; // 현재 레벨

    public int experienceTonextLevel = 100; // 다음 레벨까지 필요한 경험치
    
    public Slider experienceBar; // 경험치 바 슬라이더
    public TextMeshProUGUI levelText; // 레벨 텍스트
    public AbilityManager abilityManager; // 어빌리티 매니저 참조 2024.10.22 Lee 추가
    public Player player; // 플레이어 참조 2024.10.22 Lee 추가
    private void Start()
    {
        // 경험치 바와 레벨 텍스트 초기화
        UpdateExperienceBar();
        UpdateLevelText();
    }

    // 경험치 추가 함수
    public void AddExperience(int amount)
    {
        currentExperience += amount;

        // 경험치 초과 처리 및 레벨업 확인
        while (currentExperience >= experienceTonextLevel)
        {
            LevelUp();
        }

        // 경험치 바 업데이트
        UpdateExperienceBar();
    }
    
    // 경험치 바 업데이트 함수
    private void UpdateExperienceBar()
    {
        if (experienceBar != null)
        {
            StartCoroutine(AnimateExperienceBar());
        }
    }
    
    // 경험치 바 애니메이션 코루틴
    private IEnumerator AnimateExperienceBar()
    {
        float currentFill = experienceBar.value;
        float targetFill = (float)currentExperience / experienceTonextLevel;

        float duration = 0.2f; // 애니메이션 시간
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            experienceBar.value = Mathf.Lerp(currentFill, targetFill, elapsed / duration);
            yield return null;
        }

        experienceBar.value = targetFill; // 최종 값 보정
    }
    
    // 레벨 텍스트 업데이트 함수
    private void UpdateLevelText()
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + level;
        }
    }

    // 레벨업 체크 함수
    void CheckLevelUp()
    {
        if (currentExperience >= experienceTonextLevel)
        {
            LevelUp();
        }
    }

    // 레벨업 처리
    void LevelUp()
    {
        level++;
        currentExperience -= experienceTonextLevel;
        experienceTonextLevel += 50; // 레벨이 오를 떄마다 더 많은 경험치 필요
        
        UpdateExperienceBar(); // 레벨업 후 경험치 바 업데이트
        UpdateLevelText(); // 레벨업 후 레벨 텍스트 업데이트
        
        // 레벨업 시 AbilityManager 호출 2024.10.22 Lee 추가
        if (abilityManager != null)
        {
            abilityManager.ShowAbilitySelection();
        }
    }
}
