using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Weapon_AOE : Weapon
{
    public string Hellfire; // 무기 발사 사운드 키
    void Start()
    {
        base.Start();
        StartCoroutine(ShotWeapon());
    }
    
    private IEnumerator ShotWeapon() // 쿨타임 마다 count 수 만큼 무기 발사
    {
        while (true)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject spawnWeapon = ObjectPoolManager.Instance.SpawnFromPool(name, GetRandomPosition(), quaternion.identity);
                
                // Weapon_ContinuousDamage에 대미지 값 전달
                InputValue(spawnWeapon);

                // Weapon_ContinuousDamage의 지속 시간만큼 효과 발동
                spawnWeapon.GetComponent<Weapon_ContinuousDamage>().ActivateContinuousDamage(duration);
                
                // 무기 발사 사운드 재생
                PlayShotSound();
            }
            yield return new WaitForSeconds(cooldown);
        }
    }
    
    private void PlayShotSound()
    {
        if (!string.IsNullOrEmpty(Hellfire) && SoundManager.Instance != null)
        {
            // AudioSource를 동적으로 생성하여 사운드 반복 재생 관리
            GameObject soundObject = new GameObject("AOE_Hellfire_Sound");
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = true; // 반복 재생 설정
        
            if (SoundManager.Instance.audioDictionary.ContainsKey(Hellfire))
            {
                audioSource.clip = SoundManager.Instance.audioDictionary[Hellfire];
                audioSource.Play();
            }

            // 지정된 duration 후 사운드 정지 및 오브젝트 삭제
            Destroy(soundObject, duration);
        }
    }
    
    private Vector3 GetRandomPosition() // 플레이어를 기준으로 attackDistance 반경 내의 램덤 좌표를 반환
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float radius = Random.Range(0, attackDistance);

        float x = radius * Mathf.Cos(angle);
        float z = radius * Mathf.Sin(angle);

        //Vector3 randomPosition = new Vector3(playerTransform.position.x + x, 0.01f, playerTransform.position.z + z);
        Vector3 randomPosition = new Vector3(playerTransform.position.x + x, 1f, playerTransform.position.z + z);
        return randomPosition;
    }
}