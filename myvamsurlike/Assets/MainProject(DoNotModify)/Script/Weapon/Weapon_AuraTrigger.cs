using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_AuraTrigger : Weapon
{
    private List<Collider> collidersInTrigger = new List<Collider>(); // 충돌을 감지한 몬스터 or 보스를 리스트에 저장

    public void ActivateContinuousDamage()
    {
        StartCoroutine(CheckTriggerPeriodically());
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

    private IEnumerator CheckTriggerPeriodically() 
    {
        while (true)
        {
            //collidersInTrigger.RemoveAll(collider => !collider.gameObject.activeInHierarchy); 
            collidersInTrigger.RemoveAll(collider => collider == null || !collider.gameObject.activeInHierarchy); // 비활성화되어 풀로 돌아간 몬스터 or 디스트로이 된 몬스터 리스트에서 제거
            
            foreach (var collider in collidersInTrigger)
            {
                IDamageable monster = collider.GetComponent<IDamageable>();
                if (monster != null)
                {
                    monster.GetDamaged(damage * 0.1f);
                }
            }

            yield return new WaitForSeconds(0.1f); // 1초 간격으로 대미지 적용
        }
    }
}