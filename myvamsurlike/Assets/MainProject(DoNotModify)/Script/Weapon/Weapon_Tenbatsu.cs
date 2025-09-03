using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Tenbatsu : Weapon
{
    public string Tenbatsu;
    public string Tenbatsulaunch;

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
                // Launch 사운드 재생
                SoundManager.Instance.PlaySound(Tenbatsulaunch);
                
                GameObject spawnWeapon = ObjectPoolManager.Instance.SpawnFromPool(name, GetRandomPosition(), Quaternion.identity);
                
                InputValue(spawnWeapon);
                yield return new WaitForSeconds(2f); // Launch 이후 2초 지연
                SoundManager.Instance.PlaySound(Tenbatsu);

                StartCoroutine(WaitForReturn(spawnWeapon, 4.5f));

            }
            yield return new WaitForSeconds(cooldown);
        }
    }
    
    private Vector3 GetRandomPosition() // 플레이어를 기준으로 attackDistance 반경 내의 램덤 좌표를 반환
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float radius = Random.Range(0, attackDistance);

        float x = radius * Mathf.Cos(angle);
        float z = radius * Mathf.Sin(angle);

        Vector3 randomPosition = new Vector3(playerTransform.position.x + x, 0.01f, playerTransform.position.z + z);
        return randomPosition;
    }
    
    IEnumerator WaitForReturn(GameObject weapon,float time)// 대기 후 풀로 리턴
    {
        yield return new WaitForSeconds(time);
        ObjectPoolManager.Instance.ReturnToPool(name, weapon);
    }
}
