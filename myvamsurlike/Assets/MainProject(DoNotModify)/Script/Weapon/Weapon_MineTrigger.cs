using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_MineTrigger : Weapon
{
    private Weapon_MineExplosion mineExplosion;
    private Coroutine timeBomb;
    private bool hasExplosion;

    private void Awake()
    {
        mineExplosion = GetComponentInChildren<Weapon_MineExplosion>();
    }

    public void OnEnable()
    {
        hasExplosion = false;
        mineExplosion.gameObject.SetActive(false);
        
        StartCoroutine(DelayedStartTimeBomb()); 
    }
    
    
    private IEnumerator DelayedStartTimeBomb() // 한 프레임 지연 후 TimeBomb 코루틴과 explosion 으로 값 전달 시작 : OnEnable 은 Start 보다 빠름으로, 최상위의 Mine 구문에서 InputValue 가 이루어진 후에 값을 전달하기 위해
    {
        yield return null; // 한 프레임 대기
        InputValue(mineExplosion.gameObject);
        timeBomb = StartCoroutine(TimeBomb(duration));
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && !hasExplosion)
        {
            hasExplosion = true;

            if (timeBomb != null)
            {
                StopCoroutine(timeBomb);
            }

            mineExplosion.gameObject.SetActive(true);
            StartCoroutine(ReturnToPool(0.25f));
        }
    }
    
     IEnumerator TimeBomb(float duration)
     {
         yield return new WaitForSeconds(duration);
         if (!hasExplosion) // 중복 활성화 방지
         {
             hasExplosion = true;
             mineExplosion.gameObject.SetActive(true);
             StartCoroutine(ReturnToPool(0.25f));
         }
     }

    IEnumerator ReturnToPool(float time)
    {
        yield return new WaitForSeconds(time);
        mineExplosion.gameObject.SetActive(false); // 자식 오브젝트 초기화, 오브젝트 풀로 리턴할때 부모가 false 되면서 될 것 같지만, 안전장치로 둠
        ObjectPoolManager.Instance.ReturnToPool(name, gameObject);
    }
}
