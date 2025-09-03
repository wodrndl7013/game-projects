using System.Collections;
using UnityEngine;

public class TriggerEnter : MonoBehaviour
{
    private Player _player;
    public float damageAmount = 5f; // 몬스터와 충돌 시 줄 데미지
    public float damageInterval = 0.5f; // 데미지 간격 (초)

    private Coroutine damageCoroutine;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 몬스터 또는 보스와 충돌 시작, 코루틴이 없을 때만 실행
        if ((other.CompareTag("Monster") || other.CompareTag("Boss")) && damageCoroutine == null)
        {
            damageCoroutine = StartCoroutine(DealDamageOverTime(other));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 몬스터 또는 보스와 충돌 종료, 실행 중인 코루틴이 있을 경우 중지
        if ((other.CompareTag("Monster") || other.CompareTag("Boss")) && damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null; // 참조 초기화
        }
    }

    private IEnumerator DealDamageOverTime(Collider other)
    {
        // 충돌 중일 때 지속적으로 데미지를 입힘
        while (other != null && other.gameObject.activeInHierarchy)
        {
            _player.TakeDamage(damageAmount); // 플레이어에게 데미지 전달

            yield return new WaitForSeconds(damageInterval); // 데미지 간격 대기
        }

        // 충돌이 종료되면 코루틴을 종료
        damageCoroutine = null; // 코루틴 참조 초기화
    }
}