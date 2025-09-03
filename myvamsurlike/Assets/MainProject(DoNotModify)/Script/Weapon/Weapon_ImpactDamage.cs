using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_ImpactDamage : Weapon
{
    void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.GetDamaged(damage);
        }
    }
}
