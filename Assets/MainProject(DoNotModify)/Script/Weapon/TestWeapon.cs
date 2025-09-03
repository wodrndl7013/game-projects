using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWeapon : Weapon
{
    void Start()
    {
        StartCoroutine(FindTarget());
    }
    
    private IEnumerator FindTarget() // 거리에 상관없이 사정거리 내의 램덤한 몬스터를 목표로 하는 로직
    {
        while (true) // 쿨타임마다 반복 실행
        {
            List<GameObject> potentialTargets = new List<GameObject>();
    
            // 1. 사정거리 내 몬스터를 찾아 리스트에 추가
            foreach (var instanceMonster in MonsterSpawner.Instance.GetMonsterList())
            {
                if (instanceMonster == null) continue;
    
                var monster = instanceMonster.GetComponent<Monster>();
                var boss = instanceMonster.GetComponent<Boss>();
    
                // Monster 또는 Boss가 존재하고, isDead가 false인 경우에만 추가
                if ((monster != null && !monster.isDead) || (boss != null && !boss.isDead))
                {
                    float distanceToPlayer = (instanceMonster.transform.position - WeaponManager.Instance._player.transform.position).magnitude;
    
                    if (attackDistance >= distanceToPlayer)
                    {
                        potentialTargets.Add(instanceMonster);
                    }
                }
            }
    
            // 사정거리 내에 타겟이 있을 경우
            if (potentialTargets.Count > 0)
            {
                List<GameObject> targets = new();
                
                if (potentialTargets.Count < count) // 사정거리 내의 타겟이 설정 가능 목표 수 보다 적을 경우
                {
                    targets = potentialTargets.GetRange(0, Mathf.Min(count, potentialTargets.Count));
    
                    // 타겟의 수가 발사할 총알 갯수(count)보다 적을 경우, 타겟 중 랜덤 지정하여 남은 갯수 채우기
                    int extraBullets = count - targets.Count;
                    for (int i = 0; i < extraBullets; i++)
                    {
                        if (targets.Count > 0)
                        {
                            targets.Add(targets[Random.Range(0, targets.Count)]);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (potentialTargets.Count == 0) break; // 모든 타겟이 이미 선택된 경우 종료

                        // potentialTargets에서 랜덤한 타겟 선택
                        GameObject randomTarget = potentialTargets[Random.Range(0, potentialTargets.Count)];
                        targets.Add(randomTarget);

                        // 같은 타겟이 다시 선택되지 않도록 리스트에서 제거
                        potentialTargets.Remove(randomTarget);
                    }
                }
                
                // 타겟으로 설정한 몬스터에 대해 로직 수행
                foreach (var target in targets)
                {
                    //ShotWeapon(target);
                }
    
                yield return new WaitForSeconds(cooldown); // 쿨타임 동안 대기한 후 다음 타겟 탐색
            }
            else // 타겟 없을 경우 계속 탐색
            {
                yield return null;
            }
        }
    }
}