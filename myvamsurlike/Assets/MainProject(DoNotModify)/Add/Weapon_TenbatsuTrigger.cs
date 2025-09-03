using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_TenbatsuTrigger : Weapon
{
    private Weapon_ImpactDamage tenbatusExplosion;

    private void Awake()
    {
        tenbatusExplosion = GetComponentInChildren<Weapon_ImpactDamage>();
    }

    public void OnEnable()
    {
        tenbatusExplosion.gameObject.SetActive(false);
        
        StartCoroutine(DelayedStartTenbatsu(2f)); 
    }

    IEnumerator DelayedStartTenbatsu(float time)
    {
        yield return null; // 한 프레임 대기
        InputValue(tenbatusExplosion.gameObject);
        yield return new WaitForSeconds(time);
        tenbatusExplosion.gameObject.SetActive(true);
    }
}
