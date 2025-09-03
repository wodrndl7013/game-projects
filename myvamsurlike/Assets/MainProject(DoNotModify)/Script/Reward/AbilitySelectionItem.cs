using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AbilitySelectionItem : ItemBase
{
    protected override void ApplyEffect(GameObject player)
    {
        AbilityManager abilityManager = FindObjectOfType<AbilityManager>();
        if (abilityManager != null)
        {
            abilityManager.ShowAbilitySelection();
        }
        else
        {
            Debug.LogWarning("AbilityManager가 씬에 존재하지 않습니다.");
        }
    }
}
