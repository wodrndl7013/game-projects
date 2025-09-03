using System.Collections;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public float maxHealth = 5f;
    private float currentHealth;

    public GameObject[] debrisPrefabs; // 파편 프리팹 배열
    public int debrisCount = 5; // 생성할 파편 수
    public float explosionForce = 10f; // 폭발 힘
    public float explosionRadius = 2f; // 폭발 반경

    public Sprite normalSprite; // 정상 상태 스프라이트
    public Sprite halfSprite; // 체력이 절반일 때
    public Sprite deadlySprite; // 체력이 10퍼센트 아래일 때

    public Sprite[] smokeSprites; // 연기 스프라이트 배열
    public float smokeDuration = 1f; // 연기 효과 지속 시간

    public AudioClip destructionSound; // 파괴 음향 효과

    private SpriteRenderer spriteRenderer;
    private bool isDestroyed = false; // 오브젝트가 파괴되었는지 여부를 추적
    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 오디오 소스를 추가하고 초기화
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false; // 플레이 온 어웨이크 해제
        audioSource.spatialBlend = 0f; // 2D 사운드로 설정
        audioSource.volume = 1f; // 볼륨 설정

        UpdateSprite();

        // 디버그 로그로 debrisPrefabs 상태 확인
        Debug.Log($"debrisPrefabs is assigned: {debrisPrefabs != null}");
        if (debrisPrefabs != null)
        {
            Debug.Log($"debrisPrefabs length: {debrisPrefabs.Length}");
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDestroyed) return; // 이미 파괴된 경우 데미지 무시

        currentHealth -= damage;
        Debug.Log($"Current Health: {currentHealth}");
        UpdateSprite();
        CreateDebris(); // 체력 변화시 파편 생성

        if (currentHealth <= 0)
        {
            StartCoroutine(PlaySmokeAndDestroy());
        }
    }

    private void UpdateSprite()
    {
        Debug.Log($"Updating sprite based on current health: {currentHealth}");

        if (currentHealth >= maxHealth * 0.5f)
        {
            spriteRenderer.sprite = normalSprite;
        }
        else if (currentHealth < maxHealth * 0.5f && currentHealth >= maxHealth * 0.1f)
        {
            spriteRenderer.sprite = halfSprite;
        }
        else if (currentHealth < maxHealth * 0.1f)
        {
            spriteRenderer.sprite = deadlySprite;
        }
    }

    private IEnumerator PlaySmokeAndDestroy()
    {
        isDestroyed = true; // 오브젝트가 파괴되었음을 표시
        Debug.Log("Playing smoke and destroying object");

        // 연기 스프라이트 순차적으로 표시
        if (smokeSprites.Length > 0)
        {
            for (int i = 0; i < smokeSprites.Length; i++)
            {
                spriteRenderer.sprite = smokeSprites[i];
                yield return new WaitForSeconds(smokeDuration / smokeSprites.Length);
            }
        }

        // 파괴 음향 효과 재생
        if (destructionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(destructionSound);
            Debug.Log("Playing destruction sound");
        }

        // 사운드가 끝난 후 오브젝트 제거 (사운드가 끝나는 시간을 고려)
        Destroy(gameObject, destructionSound.length);

        // 파편 생성 및 폭발 효과 적용
        CreateDebris();
    }

    private void CreateDebris()
    {
        // debrisPrefabs 배열이 비어있는 경우 처리
        if (debrisPrefabs == null || debrisPrefabs.Length == 0)
        {
            Debug.LogWarning("debrisPrefabs array is empty or not assigned.");
            return;
        }

        // 파편 생성 및 폭발 효과 적용
        for (int i = 0; i < debrisCount; i++)
        {
            int randomIndex = Random.Range(0, debrisPrefabs.Length);
            Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0); // 오프셋 추가

            GameObject debris = Instantiate(debrisPrefabs[randomIndex], transform.position + offset, Quaternion.identity);
            Debug.Log($"Created debris {i + 1}/{debrisCount} at position {debris.transform.position}");

            Rigidbody2D rb = debris.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 explosionDirection = (rb.position - (Vector2)transform.position).normalized;
                float explosionDistance = Vector2.Distance(rb.position, transform.position);
                float explosionImpact = Mathf.Clamp01(1 - (explosionDistance / explosionRadius));

                rb.AddForce(explosionDirection * explosionForce * explosionImpact, ForceMode2D.Impulse);
            }

            // 3초 뒤 파편 제거
            Destroy(debris, 3f);
        }
    }
}
