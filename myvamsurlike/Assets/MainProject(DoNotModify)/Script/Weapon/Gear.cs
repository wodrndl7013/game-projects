using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Gear : MonoBehaviour
{
    public float rate;

    public abstract void UpgradeBroadcast(); 
}