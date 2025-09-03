using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpChecker : MonoBehaviour
{
   public ExperienceManager experienceManager; // ExperiencManager 참조
   public AbilityManager abilityManager; // AbilityManager 참조

   private int previousLevel;

   private void Start()
   {
      //게임 시작 시 현재 레벨을 저장
      previousLevel = experienceManager.level;
   }

   private void Update()
   {
      // 현재 레벨이 이전 레벨보다 높아졌는지 체크
      if (experienceManager.level > previousLevel)
      {
         // 레벨업이 발생했을 때 AbilityManager에 알림
         abilityManager.ShowAbilitySelection();
         
         // 이전 레벨 값을 현재 레벨로 업데이트
         previousLevel = experienceManager.level;
      }
   }
}
