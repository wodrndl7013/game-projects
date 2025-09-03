using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Aura : Weapon
{
    public GameObject Aura;
    public GameObject AuraEffect;
    private Weapon_AuraTrigger _aura;
    
    void Start()
    {
        base.Start();

        InputValue(Aura);
        if (Aura != null)
        {
            _aura = Aura.GetComponent<Weapon_AuraTrigger>();
        }
        SettingScale(count);
        _aura.ActivateContinuousDamage();
    }

    public override void LevelUpSetting()
    {
        base.LevelUpSetting();
        InputValue(Aura);
        SettingScale(count);
    }

    private void SettingScale(int scale)
    {
        if (_aura != null)
        {
            _aura.gameObject.transform.localScale = new Vector3(scale, 0.1f, scale);
            AuraEffect.transform.localScale = new Vector3(scale, scale, 5);
        }
        else // !!! 첫 레벨업 세팅 시 순서 문제로 이 스크립트의 start 가 불리지 않은 상태에서 LevelUpSetting 이 불려옴으로, 첫 SettingScale 의 경우 else 문으로 빠지게 하여 오류를 예방
        {
            Debug.LogWarning("Warning: _aura가 null입니다. Aura 오브젝트가 Weapon_AuraTrigger 컴포넌트를 가지고 있는지 확인하세요.");
        }
    }
}
