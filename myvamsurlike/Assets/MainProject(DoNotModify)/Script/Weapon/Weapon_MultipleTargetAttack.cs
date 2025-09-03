using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_MultipleTargetAttack : Weapon
{
    public string Thunder;
    void Start()
    {
        base.Start();
        StartCoroutine(FindTarget());
    }

    private IEnumerator FindTarget() // 가까운 타겟을 우선으로 목표로 잡는 로직
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
                    float distanceToPlayer = (instanceMonster.transform.position - playerTransform.transform.position).magnitude;

                    if (attackDistance >= distanceToPlayer)
                    {
                        potentialTargets.Add(instanceMonster);
                    }
                }
            }

            // 사정거리 내에 타겟이 있을 경우
            if (potentialTargets.Count > 0)
            {
                // 2. 플레이어로부터 먼 순서대로 정렬
                potentialTargets.Sort((a, b) =>
                    (b.transform.position - playerTransform.transform.position).sqrMagnitude.CompareTo((a.transform.position - playerTransform.transform.position).sqrMagnitude)
                );

                // 3. count 개수만큼 타겟 설정
                List<GameObject> targets = potentialTargets.GetRange(0, Mathf.Min(count, potentialTargets.Count));

                // 타겟의 수가 발사할 총알 갯수(count)보다 적을 경우, 타겟 중 랜덤 지정하여 남은 갯수 채우기
                int extraBullets = count - targets.Count;
                for (int i = 0; i < extraBullets; i++)
                {
                    if (targets.Count > 0)
                    {
                        targets.Add(targets[Random.Range(0, targets.Count)]);
                    }
                }

                // 타겟으로 설정한 몬스터에 대해 로직 수행
                foreach (var target in targets)
                {
                    ShotWeapon(target);
                }

                yield return new WaitForSeconds(cooldown); // 쿨타임 동안 대기한 후 다음 타겟 탐색
            }
            else // 타겟 없을 경우 계속 탐색
            {
                yield return null;
            }
        }
    }

    private void ShotWeapon(GameObject target)
    {
        SoundManager.Instance.PlaySound(Thunder);
        GameObject spawnWeapon = ObjectPoolManager.Instance.SpawnFromPool(name, target.transform.position, Quaternion.identity);
        
        target.GetComponent<IDamageable>().GetDamaged(damage); // 직접 데미지 가함
        
        StartCoroutine(ReturnWeapon(spawnWeapon));
    }

    private IEnumerator ReturnWeapon(GameObject weapon) // 일정 시간 대기 후 리턴
    {
        yield return new WaitForSeconds(0.5f);
        ObjectPoolManager.Instance.ReturnToPool(name, weapon);
    }
}
