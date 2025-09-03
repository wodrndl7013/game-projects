using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWeapon_Bullet : BasicWeapon
{
    private bool isMoving = false;  // 이동 중 여부 체크
    
    void Awake()
    {
        base.Awake();
    }
    
    public override void SetPositionInfo(Vector3 mousePos, Vector3 spawnPos) // 무기 스폰 위치
    {
        startPosition = spawnPos;
        moveDirection = (mousePos - spawnPos).normalized;
        isMoving = true;
    }
    
    private void MoveTowardsTarget() // 무기 이동 로직
    {
        // 처음 설정된 방향으로 이동
        transform.position += moveDirection * (Info.Speed * Time.deltaTime);

        // 지정된 AttackDistance만큼 이동했으면 풀로 반환
        if (Vector3.Distance(startPosition, transform.position) >= Info.AttackDistance)
        {
            isMoving = false;
            ObjectPoolManager.Instance.ReturnToPool(projectileType, gameObject);
        }
    }
    
    private void FixedUpdate()
    {
        if (isMoving)
        {
            MoveTowardsTarget();
        }
    }
}
