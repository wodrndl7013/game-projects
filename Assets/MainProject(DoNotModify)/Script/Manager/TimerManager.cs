using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class TimerManager : MonoBehaviour
{
    public TextMeshProUGUI timerText; // 게임에 표시할 타이머 텍스트
    private float currentTime = 0f; // 시작시간 = 0
    private int currentWave = 0; // 현재 웨이브
    private bool alreadyExistBoss = false; // 보스가 이미 게임중에 존재하는지 확인
    private int spawnValueChanger = 1; // 30초가 흐름을 체크
    private int eliteCount = 0; // 엘리트 몬스터 웨이브 카운트
    
    private void Awake()
    {
        Setting();
        StartCoroutine(CountDown());
    }

    void Setting()
    {
        currentTime = 0f;
        currentWave = 0;
        alreadyExistBoss = false;
        spawnValueChanger = 0;
        UpdateTimerText();
    }

    private IEnumerator CountDown()
    {
        // while (currentTime < 1200) // 20분이 되기 전까지 루프
        // {
        //     yield return new WaitForSeconds(1f);
        //     currentTime++;
        //     spawnValueChanger++;
        //     UpdateTimerText();
        //     
        //     if (Mathf.Approximately(currentTime, 1140) && !alreadyExistBoss) // 18분일 때 보스 소환 밑 웨이브 고정
        //     {
        //         SpawnBoss();
        //     }
        //
        //     if (currentTime % 120 == 0 && !alreadyExistBoss) // 2분마다 엘리트 소환, 16분까지 = 총 8명
        //     {
        //         SpawnElite();
        //     }
        //     
        //     if (currentTime % 180 == 0 && !alreadyExistBoss) // 3분마다 웨이브 변경
        //     {
        //         ChangeWave();
        //     }
        //     
        //     if (spawnValueChanger == 30 && !alreadyExistBoss) // 30초마다 스폰 비율 변경
        //     {
        //         UpdateSpawnRates();
        //         spawnValueChanger = 0; // 다시 0으로 리셋
        //     }
        // }
        // GameClear();
        
        while (currentTime < 1200) // 20분이 되기 전까지 루프
        {
            yield return new WaitForSeconds(1f);
            currentTime++;
            spawnValueChanger++;
            UpdateTimerText();
            
            if (Mathf.Approximately(currentTime, 720) && !alreadyExistBoss) // 12분일 때 보스 소환 밑 웨이브 고정
            {
                SpawnBoss();
            }

            if (currentTime % 75 == 0 && !alreadyExistBoss) // 1분 15초마다 엘리트 소환, 10분까지 = 총 8명
            {
                SpawnElite();
            }
            
            if (currentTime % 120 == 0 && !alreadyExistBoss) // 2분마다 웨이브 변경
            {
                ChangeWave();
            }
            
            if (spawnValueChanger == 20 && !alreadyExistBoss) // 30초마다 스폰 비율 변경
            {
                UpdateSpawnRates();
                spawnValueChanger = 0; // 다시 0으로 리셋
            }
        }
        GameClear();
    }

    void UpdateTimerText() // 타이머 텍스트 업데이트
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); // 00:00 형식으로 출력
    }
    
    void ChangeWave()
    {
        MonsterSpawner.Instance.firstWave = false; // 스포너에 첫번째 웨이브가 지나갔음을 알림
        
        currentWave++;
        MonsterSpawner.Instance.SetWave(currentWave); // 스포너에 웨이브 변경 요청
    }
    
    void UpdateSpawnRates()
    {
        //MonsterSpawner.Instance.UpdateSpawnRates(currentTime % 180); // 3분 주기로 스폰 비율 변경
        MonsterSpawner.Instance.UpdateSpawnRates(currentTime % 120); // 2분 주기로 스폰 비율 변경
    }

    void SpawnBoss() // 보스 몬스터 소환
    {
        MonsterSpawner.Instance.SpawnBossMonster();
        alreadyExistBoss = true; // 보스가 소환되었음을 알림
    }

    void SpawnElite()
    {
        MonsterSpawner.Instance.SpawnEliteMonster(eliteCount);
        eliteCount++;
    }

    void GameClear() // !! 미완성 : 게임 클리어 이후 나타날 로직 = 시간제한이 되었을 때 해당 로직이 작동 or 보스 몬스터 Dead 에 다른 클리어 로직 추가
    { 
        // 무한루프 코루틴 멈추고, 다음 스테이지로 갈 수 있게 로직 구성
        Debug.Log("스테이지 클리어");
    }
}

