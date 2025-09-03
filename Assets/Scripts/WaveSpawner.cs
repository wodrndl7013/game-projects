using System;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour // WaveSpawner 클래스 정의, MonoBehaviour를 상속받음
{
    public Transform enemyPrefab; // 적 프리팹을 저장할 변수 선언

    public Transform spawnPoint;

    public float timeBetweenWaves = 5f; // 웨이브 사이의 시간 간격 설정 (5초)
    private float countdown = 2.5f;  // 다음 웨이브까지 남은 시간을 저장할 변수 (2초)

    public TextMeshProUGUI waveCountdownText;
    private int waveIndex = 0;
    void Update() // 매 프레임마다 실행되는 Update 메서드
    {
        if (countdown <= 0f) // 카운트다운이 0 이하로 떨어지면
        {
            StartCoroutine(SpawnWave()); // 새로운 웨이브 생성
            countdown = timeBetweenWaves; // 카운트다운을 다시 웨이브 간 시간으로 리셋
        }

        countdown -= Time.deltaTime; // 카운트다운 감소 (프레임 간 경과 시간만큼)

        waveCountdownText.text = Mathf.Round(countdown).ToString();
    }

    IEnumerator SpawnWave() // 웨이브를 생성하는 메서드
    {
        waveIndex++;
        
        for (int i = 0; i < waveIndex; i++)
        {
            spawnEnemy();
            yield return new WaitForSeconds(0.5f);
        }
    }

    void spawnEnemy()
    {
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
