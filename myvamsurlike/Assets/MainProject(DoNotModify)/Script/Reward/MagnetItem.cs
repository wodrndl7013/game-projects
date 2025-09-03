// MagnetItem.cs
using System.Collections;
using UnityEngine;

public class MagnetItem : ItemBase
{
    protected override void ApplyEffect(GameObject player)
    {
        // 맵에 있는 경험치 오브만 가져오기
        GameObject[] experienceOrbs = GameObject.FindGameObjectsWithTag("Exp");

        foreach (GameObject orb in experienceOrbs)
        {
            if (orb != null)
            {
                // 경험치 오브의 이동 기능을 직접 호출
                ExperienceOrb experienceOrb = orb.GetComponent<ExperienceOrb>();
                if (experienceOrb != null)
                {
                    experienceOrb.ActivateMagnetEffect(player.transform);
                }
            }
        }
    }
}