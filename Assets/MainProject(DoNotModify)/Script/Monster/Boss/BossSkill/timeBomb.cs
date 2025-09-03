using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timeBomb : MonoBehaviour
{
    public float damageAmount = 20f; // 폭탄이 줄 데미지 양
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.TakeDamage(damageAmount);
        }
    }
    
    private void OnEnable()
    {
        EffectManager.Instance.PlayEffect(EffectType.Explosion2, transform.position);
        SoundManager.Instance.PlayExplosionSound();
    }
}