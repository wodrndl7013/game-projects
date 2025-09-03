using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterSpawner : Singleton<MonsterSpawner>
{
    public float innerRadius = 20f;      // 스폰이 안되는 내부 반지름
    public float outerRadius = 30f;     // 스폰 가능한 외부 반지름
    public int spawnCount = 1;          // 초당 생성할 오브젝트 수
    public float spawnInterval = 1f;    // 오브젝트 생성 간격 (초당)
    private Transform movingObject;      // 기준이 되는 계속 움직이는 오브젝트(플레이어)

    [NonSerialized] public Dictionary<int, GameObject> MonsterInstance = new ();
    
    public GameObject boss;
    public List<GameObject> eliteList;
    
    public List<PooledObjectData> monsterList; // 오브젝트 풀에서 몬스터만 들고 와서 저장
    // 웨이브마다 소환할 몬스터 두 종류
    public PooledObjectData monsterA; 
    public PooledObjectData monsterB;
    // 소환 확률을 결정할 두 변수
    private float spawnRateA = 1f;
    private float spawnRateB = 0f;
    public bool firstWave = true; // 첫 웨이브인지 확인

    private void Awake()
    {
        SettingPlayer();
    }
    
    private void Start()
    {
        SettingList();
        StartCoroutine(SpawnObjects());
    }

    void SettingPlayer()
    { 
        movingObject = GameObject.FindWithTag("Player").transform;
    }
    
    void SettingList() // 풀 매니저에 등록된 Data 중 Monster 만 가져와 저장
    {
        // 풀매니저에 등록된 프리팹 리스트에서 Mosnter 로 시작되는 것들만 빼옴
        List<PooledObjectData> allPooledObjectDatas = ObjectPoolManager.Instance.objectsToPool;
        if (allPooledObjectDatas != null)
        {
            // monsterList 설정
            monsterList = allPooledObjectDatas.FindAll(obj => obj.objectTypeName.StartsWith("Monster"));
            
            monsterList.Sort((a, b) =>
            {
                string[] aSplit = a.objectTypeName.Split('_');
                string[] bSplit = b.objectTypeName.Split('_');

                int aNumber = int.Parse(aSplit[1]);
                int bNumber = int.Parse(bSplit[1]);

                return aNumber.CompareTo(bNumber);
            }); // _ 이후 숫자 순으로 정렬
            
            SetWave(0); // 첫번째 웨이브 설정은 여기서, 이후 웨이브는 TimerManger 에서 이루어짐
        }
    }
    
    IEnumerator SpawnObjects() // 몬스터를 스폰시키는 코루틴
    {
        while (true)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                GameObject spawnedMonster = GetRandomMonster();
                if (spawnedMonster != null)
                {
                    AddMonsterDictionary(spawnedMonster);
                }
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    private GameObject GetRandomMonster() // 스폰할 몬스터를 확률에 기반하여 선택
    {
        float totalRate = spawnRateA + spawnRateB;
        float randomValue = Random.Range(0f, totalRate);

        if (randomValue <= spawnRateA)
        {
            return ObjectPoolManager.Instance.SpawnFromPool(monsterA.objectTypeName, GetRandomPositionInDonut(), Quaternion.identity);
        }
        else
        {
            return ObjectPoolManager.Instance.SpawnFromPool(monsterB.objectTypeName, GetRandomPositionInDonut(), Quaternion.identity);
        }
    }
    
    public void SetWave(int wave) // 각 웨이브에 맞는 두 마리 몬스터를 할당 (2마리씩 차례대로)
    {
        int firstMonsterIndex = (wave * 2) % monsterList.Count;
        int secondMonsterIndex = (firstMonsterIndex + 1) % monsterList.Count;

        monsterA = monsterList[firstMonsterIndex];
        monsterB = monsterList[secondMonsterIndex];
    }
    
    public void UpdateSpawnRates(float elapsedWaveTime) // 경과된 시간에 따라 스폰 비율을 변경
    {
        // if (elapsedWaveTime < 30f && firstWave)
        // {
        //     spawnRateA = 1f;
        //     spawnRateB = 0f;
        // }
        // else if (elapsedWaveTime < 30f)
        // {
        //     spawnRateA = 3f;
        //     spawnRateB = 1f;
        // }
        // else if (elapsedWaveTime < 90f)
        // {
        //     spawnRateA = 2f;
        //     spawnRateB = 1f;
        // }
        // else if (elapsedWaveTime < 150f)
        // {
        //     spawnRateA = 1f;
        //     spawnRateB = 1f;
        // }
        // else
        // {
        //     spawnRateA = 1f;
        //     spawnRateB = 2f;
        // }
        if (elapsedWaveTime < 20f && firstWave)
        {
            spawnRateA = 1f;
            spawnRateB = 0f;
        }
        else if (elapsedWaveTime < 20f)
        {
            spawnRateA = 3f;
            spawnRateB = 1f;
        }
        else if (elapsedWaveTime < 60f)
        {
            spawnRateA = 2f;
            spawnRateB = 1f;
        }
        else if (elapsedWaveTime < 100f)
        {
            spawnRateA = 1f;
            spawnRateB = 1f;
        }
        else
        {
            spawnRateA = 1f;
            spawnRateB = 2f;
        }
    }
    
    private Vector3 GetRandomPositionInDonut() // 도넛 모양 범위 내에서 랜덤한 위치 반환
    {
        Vector3 randomPosition;

        // 무작위 위치를 얻을 때까지 반복
        do
        {
            // 외부 반지름 안의 무작위 위치를 얻음
            Vector3 randomDirection = Random.insideUnitSphere * outerRadius;
            randomDirection.y = 0; // 2D 또는 평면에서의 생성이면 Y축을 0으로 고정
            randomPosition = movingObject.position + randomDirection;
        }
        // 무작위 위치가 내부 반지름 바깥쪽인 경우에만 위치 반환
        while (Vector3.Distance(movingObject.position, randomPosition) < innerRadius);

        return randomPosition;
    }

    private void AddMonsterDictionary(GameObject monster) // 딕셔너리에 생성된 몬스터 추가
    {
        MonsterInstance.Add(monster.GetInstanceID(), monster);
    }

    public List<GameObject> GetMonsterList() // 몬스터 값만 리스트로 반환 : 추후에 타겟 설정을 위해 사용
    {
        return MonsterInstance.Values.ToList();
    }

    public void RemoveMonster(int instanceID) // 죽은 몬스터를 딕셔너리에서 제거하고 풀로 반환
    {
        if (MonsterInstance.ContainsKey(instanceID))
        {
            GameObject monster = MonsterInstance[instanceID];
            
            Monster monsterComponent = monster.GetComponent<Monster>();
            
            if (monsterComponent.MonsterType == MonsterType.Normal) // 노말타입 몬스터의 경우, 오브젝트 풀로 Return 시키는 로직.
            {
                string monsterName = monster.name.Replace("(Clone)", ""); // 오브젝트가 "프리팹이름(clone)" 으로 생성됨으로, (clone)부분을 지워 원본과 같은 이름을 만들어 받아옴
                ObjectPoolManager.Instance.ReturnToPool(monsterName, monster); // 풀로 반환 = 프리팹 이름으로 하므로 PooledObjectData 의 objectTypeName 이 프리팹 이름과 맞아야 함.
            }
            else // 그 외 엘리트, 보스 몬스터는 Destroy
            {
                Destroy(monster.gameObject);
            }
            
            MonsterInstance.Remove(instanceID); 
        }
    }
    
    public void SpawnBossMonster() // 보스 몬스터 소환 로직
    {
        Vector3 spawnPosition = GetRandomPositionInDonut();
        GameObject spawnBoss = Instantiate(boss, spawnPosition, Quaternion.identity);
        AddMonsterDictionary(spawnBoss);
    }

    public void SpawnEliteMonster(int index) // 엘리트 몬스터 소환 로직
    {
        if (index < eliteList.Count)
        {
            Vector3 spawnPosition = GetRandomPositionInDonut();
            GameObject spawnElite = Instantiate(eliteList[index], spawnPosition, Quaternion.identity);
            AddMonsterDictionary(spawnElite);
        }
    }
}