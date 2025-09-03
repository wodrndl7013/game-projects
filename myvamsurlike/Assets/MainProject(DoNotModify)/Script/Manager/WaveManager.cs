using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public string monsterPoolType = "Monster_13"; // 몬스터 풀 타입
    public Transform player; // 플레이어 위치
    public int enemiesPerWave = 20; // 웨이브당 몬스터 수

    public enum SpawnPattern
    {
        Circular,
        Diamond,
        TopEdge,
        RightEdge,
        BottomEdge,
        LeftEdge
    }

    public SpawnPattern currentPattern = SpawnPattern.Circular;

    private float sceneLoadTime; // 씬 로드 시간을 저장

    void Start()
    {
        // 씬 로드 시간 저장
        sceneLoadTime = Time.time;

        // 플레이어를 찾기
        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player 태그를 가진 오브젝트를 찾을 수 없습니다. Inspector에서 player를 수동으로 할당하세요.");
                return;
            }
        }

        // 웨이브 시작
        StartCoroutine(WaveRoutine());
    }

    IEnumerator WaveRoutine()
    {
        while (true)
        {
            float elapsedTime = Time.time - sceneLoadTime; // 경과 시간 계산

            // 1분 30초 간격으로 웨이브 발생
            if (elapsedTime >= 90f && (elapsedTime % 90f) < 1f)
            {
                // 현재 패턴에 따라 스폰 실행
                switch (currentPattern)
                {
                    case SpawnPattern.Circular:
                        yield return StartCoroutine(SpawnCircularWave());
                        break;
                    case SpawnPattern.Diamond:
                        yield return StartCoroutine(SpawnDiamondWave());
                        break;
                    case SpawnPattern.TopEdge:
                        yield return StartCoroutine(SpawnEdgeWave(new Vector3(-10, 0, 10), new Vector3(10, 0, 10)));
                        break;
                    case SpawnPattern.RightEdge:
                        yield return StartCoroutine(SpawnEdgeWave(new Vector3(10, 0, 10), new Vector3(10, 0, -10)));
                        break;
                    case SpawnPattern.BottomEdge:
                        yield return StartCoroutine(SpawnEdgeWave(new Vector3(10, 0, -10), new Vector3(-10, 0, -10)));
                        break;
                    case SpawnPattern.LeftEdge:
                        yield return StartCoroutine(SpawnEdgeWave(new Vector3(-10, 0, -10), new Vector3(-10, 0, 10)));
                        break;
                }

                // 패턴 변경 순서대로 이동
                currentPattern = (SpawnPattern)(((int)currentPattern + 1) % System.Enum.GetValues(typeof(SpawnPattern)).Length);
            }

            // 대기 시간
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator SpawnCircularWave()
    {
        float radius = 10f;
        float angleStep = 360f / enemiesPerWave;

        for (int i = 0; i < enemiesPerWave; i++)
        {
            float angle = i * angleStep;
            Vector3 spawnPosition = new Vector3(
                player.position.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad),
                player.position.y,
                player.position.z + radius * Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            // 풀에서 몬스터 스폰
            GameObject monster = ObjectPoolManager.Instance.SpawnFromPool(monsterPoolType, spawnPosition, Quaternion.identity);
            if (monster != null)
            {
                Monster monsterComponent = monster.GetComponent<Monster>();
                if (monsterComponent != null)
                {
                    // OnDestroyed 이벤트 등록
                    monsterComponent.RegisterOnDestroyed(() =>
                    {
                        ObjectPoolManager.Instance.ReturnToPool(monsterPoolType, monster);
                    });
                }
            }

            yield return null; // 스폰 간격 유지
        }
    }

    IEnumerator SpawnDiamondWave()
    {
        float size = 10f; // 마름모 크기
        int segments = enemiesPerWave / 4; // 각 면에 배치될 몬스터 수

        // 상단 변
        for (int i = 0; i <= segments; i++)
        {
            Vector3 spawnPosition = new Vector3(
                player.position.x - size + (i * (size * 2) / segments),
                player.position.y,
                player.position.z + size - (Mathf.Abs(i - segments / 2) * (size / segments))
            );
            SpawnMonster(spawnPosition);
            yield return null;
        }

        // 우측 변
        for (int i = 0; i <= segments; i++)
        {
            Vector3 spawnPosition = new Vector3(
                player.position.x + size - (Mathf.Abs(i - segments / 2) * (size / segments)),
                player.position.y,
                player.position.z - size + (i * (size * 2) / segments)
            );
            SpawnMonster(spawnPosition);
            yield return null;
        }

        // 하단 변
        for (int i = 0; i <= segments; i++)
        {
            Vector3 spawnPosition = new Vector3(
                player.position.x + size - (i * (size * 2) / segments),
                player.position.y,
                player.position.z - size + (Mathf.Abs(i - segments / 2) * (size / segments))
            );
            SpawnMonster(spawnPosition);
            yield return null;
        }

        // 좌측 변
        for (int i = 0; i <= segments; i++)
        {
            Vector3 spawnPosition = new Vector3(
                player.position.x - size + (Mathf.Abs(i - segments / 2) * (size / segments)),
                player.position.y,
                player.position.z + size - (i * (size * 2) / segments)
            );
            SpawnMonster(spawnPosition);
            yield return null;
        }
    }

    IEnumerator SpawnEdgeWave(Vector3 start, Vector3 end)
    {
        int segments = enemiesPerWave; // 각 변에 배치될 몬스터 수

        for (int i = 0; i <= segments; i++)
        {
            Vector3 spawnPosition = Vector3.Lerp(start, end, (float)i / segments);
            SpawnMonster(spawnPosition);
            yield return null; // 스폰 간격 유지
        }
    }

    void SpawnMonster(Vector3 position)
    {
        GameObject monster = ObjectPoolManager.Instance.SpawnFromPool(monsterPoolType, position, Quaternion.identity);
        if (monster != null)
        {
            Monster monsterComponent = monster.GetComponent<Monster>();
            if (monsterComponent != null)
            {
                monsterComponent.RegisterOnDestroyed(() =>
                {
                    ObjectPoolManager.Instance.ReturnToPool(monsterPoolType, monster);
                });
            }
        }
    }
}
