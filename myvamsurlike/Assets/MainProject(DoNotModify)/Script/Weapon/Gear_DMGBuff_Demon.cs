using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear_DMGBuff_Demon : Gear
{
    public override void UpgradeBroadcast()
    {
        WeaponManager.Instance.Broadcast_DMGBUff_Demon(rate);
    }
}
