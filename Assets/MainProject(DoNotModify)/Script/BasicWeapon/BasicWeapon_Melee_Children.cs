using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWeapon_Melee_Children : MonoBehaviour
{
    private BasicWeapon_Melee parent;

    private void Start()
    {
        parent = GetComponentInParent<BasicWeapon_Melee>();
    }

    private void OnTriggerEnter(Collider other)
    {
        parent.OnTriggerEnter(other);
    }
}
