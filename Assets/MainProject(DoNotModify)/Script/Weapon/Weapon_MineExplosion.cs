using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_MineExplosion : Weapon
{
    private void OnEnable()
    {
        SettingScale();
        EffectManager.Instance.PlayEffect(EffectType.Explosion, transform.position);
        SoundManager.Instance.PlayExplosionSound();
    }

    void OnTriggerEnter(Collider other)
    {
        
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.GetDamaged(damage);
        }
    }
    
    private void SettingScale()
    {
        transform.localScale = new Vector3(count, 0.1f, count);
    }
}
