using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Weapon : MonoBehaviour
{
    public string name;
    public float damage;
    public int count;
    public float speed;
    public float attackDistance;
    public float cooldown;
    public float duration;
    public Transform playerTransform;

    public void Start()
    {
        playerTransform = WeaponManager.Instance._player.transform;
    }

    protected virtual void InputValue(GameObject spawnWeapon) // 데미지 정보를 오브젝트 풀에서 불러올 웨펀에 전달
    {
        Weapon weapon = spawnWeapon.GetComponent<Weapon>();
        weapon.damage = damage;
        weapon.duration = duration;
        weapon.name = name;
        weapon.count = count;
    }

    public virtual void LevelUpSetting()
    {
        // 레벨업시 조정이 필요한 무기들 사용(ex) FiendWheel 같이 ImpactDamage 를 순간 사용하고 풀로 돌리는 것이 아닌, 씬에 지속적으로 남아있게 하는 무기 종류의 경우)
    }
}