using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear_DMGBuff_Angel : Gear
{
    public override void UpgradeBroadcast()
    {
        WeaponManager.Instance.Broadcast_DMGBUff_Angel(rate);
    }
}
