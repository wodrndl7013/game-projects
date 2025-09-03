using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_ContinuousDamage : Weapon
{
    private List<Collider> collidersInTrigger = new List<Collider>(); // 충돌을 감지한 몬스터 or 보스를 리스트에 저장
    private Coroutine checkCoroutine;

    public void ActivateContinuousDamage(float effectDuration)
    {
        // 코루틴을 통해 지속 대미지 발동
        if (checkCoroutine == null)
        {
            checkCoroutine = StartCoroutine(CheckTriggerPeriodically(effectDuration));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster") || other.CompareTag("Boss"))
        {
            collidersInTrigger.Add(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Monster") || other.CompareTag("Boss"))
        {
            collidersInTrigger.Remove(other);
        }
    }

    private IEnumerator CheckTriggerPeriodically(float effectDuration) // 1초 간격으로 지속적으로 AOE 위에 있는 몬스터를 감지해 데미지를 적용 및 속도 느리게 함
    {
        float elapsed = 0f;
        while (elapsed < effectDuration * 10f)
        {
            foreach (var collider in collidersInTrigger)
            {
                IDamageable monster = collider.GetComponent<IDamageable>();
                if (monster != null)
                {
                    monster.GetDamaged(damage * 0.1f);
                    monster.GetSlowed(1f);
                }
            }

            yield return new WaitForSeconds(0.1f); // 1초 간격으로 대미지 적용
            elapsed += 1f;
        }

        ObjectPoolManager.Instance.ReturnToPool(name, gameObject);
    }
    
    void OnDisable() // 오브젝트가 비활성화될 때 호출됨
    {
        if (checkCoroutine != null)
        {
            StopCoroutine(checkCoroutine);
            checkCoroutine = null; // 코루틴 초기화
        }

        // 추가적으로 리스트 초기화
        collidersInTrigger.Clear();
    }
}