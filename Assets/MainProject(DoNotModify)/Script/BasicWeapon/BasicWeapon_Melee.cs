using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWeapon_Melee : BasicWeapon
{
    private bool isMoving = false; // 이동 중 여부 체크
    public string Sword;
    
    void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        StartCoroutine(WaitAndExecute(0.1f));
    }

    public override void SetPositionInfo(Vector3 mousePos, Vector3 spawnPos) // 무기 스폰 위치
    {
        SoundManager.Instance.PlaySound(Sword);
        startPosition = spawnPos;
        moveDirection = (mousePos - spawnPos).normalized;
        isMoving = true;
    }
    
    
    IEnumerator WaitAndExecute(float time)// 대기 후 풀로 리턴
    {
        yield return new WaitForSeconds(time);
        ObjectPoolManager.Instance.ReturnToPool(projectileType, gameObject);
    }
}