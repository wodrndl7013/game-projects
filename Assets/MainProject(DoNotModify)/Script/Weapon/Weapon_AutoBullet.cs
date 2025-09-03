using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Weapon_AutoBullet : Weapon
{
    public string AutoBullet; // 발사 사운드 키 추가
    void Start()
    {
        base.Start();
        StartCoroutine(FindTarget());
    }

    private IEnumerator FindTarget() // 가장 가까운 하나의 타겟만을 목표로 하는 로직
    {
        while (true) // 쿨타임마다 반복 실행
        {
            GameObject closestTarget = null; // 가장 가까운 타겟
            float closestDistance = float.MaxValue; // 거리 비교용 변수. 첫 값을 Max 로 지정해 처음 검사하는 몬스터와의 거리보다 무조건 크게 한다.

            // 1. 사정거리 내 가장 가까운 타겟 찾기
            foreach (var instanceMonster in MonsterSpawner.Instance.GetMonsterList())
            {
                if (instanceMonster == null) continue;

                var monster = instanceMonster.GetComponent<Monster>();
                var boss = instanceMonster.GetComponent<Boss>();

                // Monster 또는 Boss가 존재하고, isDead가 false인 경우에만 거리 계산
                if ((monster != null && !monster.isDead) || (boss != null && !boss.isDead))
                {
                    float distanceToPlayer = (instanceMonster.transform.position - playerTransform.transform.position).magnitude;

                    // 사정거리 내에서 가장 가까운 타겟 선택
                    if (distanceToPlayer < closestDistance && distanceToPlayer <= attackDistance)
                    {
                        closestDistance = distanceToPlayer;
                        closestTarget = instanceMonster;
                    }
                }
            }

            // 가장 가까운 타겟이 있으면 그 방향으로 산탄 발사
            if (closestTarget != null)
            {
                ShotWeapon(closestTarget); // 가장 가까운 타겟 방향으로 산탄 발사
                yield return new WaitForSeconds(cooldown); // 쿨타임 동안 대기한 후 다음 타겟 탐색
            }
            else // 타겟이 없을 경우 계속 탐색
            {
                yield return null;
            }
        }
    }

    private void ShotWeapon(GameObject target)
    {
        Vector3 startPos = playerTransform.transform.position;
        Vector3 targetPos = target.transform.position;

        // 타겟이 너무 가까운 경우 기본 방향으로 설정 (!!! 타겟이 너무 가까울 경우, 탄환이 매우 느리게 날아가는 버그 방지)
        float minDistance = 0.1f;
        Vector3 baseDirection = (targetPos - startPos).magnitude < minDistance 
            ? Vector3.right  // 기본 방향을 오른쪽으로 설정
            : (targetPos - startPos).normalized;
        
        baseDirection.y = 0; // Y축 고정

        float spreadAngle = 5f * count; // 최대 퍼뜨리는 각도
        float angleStep = (spreadAngle * 2) / (count - 1); // 각 탄환 사이의 각도 간격 !!! count 가 0일 경우 버그남. 단, 이 무기는 최소 3발이므로 예외처리 생략하겠음
        
        // 발사 사운드 재생
        if (SoundManager.Instance != null && !string.IsNullOrEmpty(AutoBullet))
        {
            SoundManager.Instance.PlaySound(AutoBullet);
        }

        for (int i = 0; i < count; i++)
        {
            // 각 탄환의 발사 각도를 일정하게 분산
            float angleOffset = -spreadAngle + (angleStep * i);
            Quaternion rotation = Quaternion.AngleAxis(angleOffset, Vector3.up);
            Vector3 spreadDirection = rotation * baseDirection;

            // 탄환 생성 및 회전 방향 설정
            Quaternion bulletRotation = Quaternion.LookRotation(spreadDirection);
            GameObject spawnWeapon = ObjectPoolManager.Instance.SpawnFromPool(name, startPos, bulletRotation);
        
            InputValue(spawnWeapon); // 대미지 전달

            // 무기 이동 코루틴 시작
            StartCoroutine(MoveWeapon(spawnWeapon, spreadDirection, startPos));
        }
    }

    private IEnumerator MoveWeapon(GameObject weapon, Vector3 direction, Vector3 startPos) // 무기 이동 코루틴
    {
        direction = direction.normalized;
        
        while (Vector3.Distance(weapon.transform.position, startPos) <= attackDistance)
        {
            weapon.transform.position += direction * (speed * Time.deltaTime);
            yield return null;
        }

        // 공격 범위를 벗어나면 풀로 반환
        ObjectPoolManager.Instance.ReturnToPool(name, weapon);
    }
}
