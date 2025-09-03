using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Mine : Weapon
{
    public string MinePlaceSound; // 마인 설치 시 재생할 사운드 이름
    void Start()
    {
        base.Start();
        StartCoroutine(SetMine());
    }

    IEnumerator SetMine()
    {
        while (true)
        {
            GameObject mine = ObjectPoolManager.Instance.SpawnFromPool(name, playerTransform.position, Quaternion.identity);
            InputValue(mine);
            
            // 사운드 재생
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySound(MinePlaceSound);
            }
            yield return new WaitForSeconds(cooldown);
        }
    }
}
