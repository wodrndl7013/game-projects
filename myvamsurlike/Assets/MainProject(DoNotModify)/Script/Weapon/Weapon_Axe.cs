using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Axe : Weapon
{
    public string Axe;
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
        Vector3 startPos = playerTransform.transform.position;
        GameObject spawnWeapon = ObjectPoolManager.Instance.SpawnFromPool(name, startPos, Quaternion.identity);
        Vector3 moveDir = (target.transform.position - startPos).normalized;
        moveDir.y = 0; // 무기가 지면아래로 꺼지거나 위로 날아가는 현상을 방지
        
        InputValue(spawnWeapon); // 대미지 전달
        
        // 무기 오브젝트에 AudioSource를 연결하여 사운드 관리
        AudioSource audioSource = spawnWeapon.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = spawnWeapon.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = true; // 반복 재생 설정
        }
        
        // 발사 사운드 재생
        if (SoundManager.Instance != null && !string.IsNullOrEmpty(Axe))
        {
            audioSource.clip = SoundManager.Instance.audioDictionary[Axe];
            audioSource.Play();
        }
        
        StartCoroutine(MoveWeapon(spawnWeapon, moveDir, startPos, audioSource));
        StartCoroutine(RotateY(spawnWeapon));
    }
    
    private IEnumerator RotateY(GameObject weapon) // weapon 회전 코루틴
    {
        while (weapon.activeInHierarchy)
        {
            // Y축을 기준으로 회전
            weapon.transform.Rotate(0, 360f * (2f * Time.deltaTime), 0);
            yield return null; // 다음 프레임까지 대기
        }
    }

    private IEnumerator MoveWeapon(GameObject weapon, Vector3 direction, Vector3 startPos, AudioSource audioSource) // 무기 이동 코루틴
    {
        while (Vector3.Distance(weapon.transform.position, startPos) <= attackDistance)
        {
            weapon.transform.position += direction * (speed * Time.deltaTime);
            yield return null;
        }

        // 공격 범위를 벗어나면 다시 돌아오는 코루틴 시작
        StartCoroutine(ComebackWeapon(weapon, playerTransform, audioSource));
    }

    private IEnumerator ComebackWeapon(GameObject weapon, Transform player, AudioSource audioSource) // 임계점에 달한 weapon player 에게 회귀
    {
        while (Vector3.Distance(weapon.transform.position, player.position) >= 1)
        {
            Vector3 directionToPlayer = (player.position - weapon.transform.position).normalized;
            weapon.transform.position += directionToPlayer * (speed * Time.deltaTime);
            yield return null;
        }
        
        // 플레이어에게 돌아오면 사운드 중지
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        
        ObjectPoolManager.Instance.ReturnToPool(name, weapon);
    }
}
