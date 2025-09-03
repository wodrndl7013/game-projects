// ItemBase.cs
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    public float detectionRange = 5f; // 플레이어 감지 범위
    public float moveSpeed = 10f; // 아이템이 날아가는 속도
    private Transform player;

    protected abstract void ApplyEffect(GameObject player);

    private void Start()
    {
        // 플레이어 위치 찾기
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // 플레이어와의 거리 계산 및 이동 처리
        MoveToPlayerIfInRange();
    }

    private void MoveToPlayerIfInRange()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            MoveToPlayer();
        }
    }

    private void MoveToPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyEffect(other.gameObject); // 플레이어에게 효과 적용
            Destroy(gameObject); // 아이템 사용 후 제거
        }
    }
}