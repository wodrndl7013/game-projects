// AbilityItem.cs
using UnityEngine;

public class AbilityItem : ItemBase
{
    protected override void ApplyEffect(GameObject player)
    {
        // 능력 선택 UI 호출
        AbilityManager abilityManager = AbilityManager.Instance;
        if (abilityManager != null)
        {
            abilityManager.ShowAbilitySelection();
        }
        else
        {
            Debug.LogWarning("AbilityManager.Instance가 설정되지 않았습니다.");
        }
    }
}