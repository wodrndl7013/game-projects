using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkill_TimeBomb : BossSkill
{
    private Transform timeBomb;
    public float time = 3f;
    public float explosionRadius = 5f; // 폭발 범위
    public GameObject explosionEffect; // 폭발 이펙트
    public UnityEngine.UI.Image timerGauge; // 게이지 UI (FillAmount로 표현)
    
    private void Awake()
    {
        base.Awake();
        timeBomb = transform.Find("timeBomb");

        if (timeBomb != null)
        {
            timeBomb.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (timeBomb != null)
        {
            timeBomb.gameObject.SetActive(false);
        }
        
        // 게이지 초기화
        if (timerGauge != null)
        {
            timerGauge.fillAmount = 0f;
        }

        StartCoroutine(TimeBombTicking());
    }

    IEnumerator TimeBombTicking()
    {
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            // 타이머 게이지 채우기
            if (timerGauge != null)
            {
                timerGauge.fillAmount = elapsedTime / time;
            }

            yield return null;
        }

        TimeBombActivating();
    }

    void TimeBombActivating()
    {
        // 폭탄을 활성화
        if (timeBomb != null)
        {
            timeBomb.gameObject.SetActive(true);
        }
        
        StartCoroutine(ReturnToPool());
    }

    IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(1f);

        // 폭탄 비활성화 (풀로 돌아가기 전에)
        if (timeBomb != null)
        {
            timeBomb.gameObject.SetActive(false);
        }

        ObjectPoolManager.Instance.ReturnToPool(projectileType, gameObject);
    }
}