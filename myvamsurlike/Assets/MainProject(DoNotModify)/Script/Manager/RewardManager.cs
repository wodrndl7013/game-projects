using UnityEngine;

public class RewardManager : Singleton<RewardManager>
{
    public GameObject experienceOrbPrefab_Basic;
    public GameObject experienceOrbPrefab_Advanced;
    public GameObject experienceOrbPrefab_Premium;
    public GameObject magnetItemPrefab;  // 자석 아이템 프리팹
    public GameObject healthItemPrefab;  // 회복 아이템 프리팹
    public GameObject abilityItemPrefab; // 능력 선택 아이템 프리팹

    private const float healthDropRate = 0.01f;   // 회복 아이템 드랍 확률 1%
    private const float magnetDropRate = 0.01f;  // 자석 아이템 드랍 확률 1%

    public void DropExperienceOrb(Vector3 position)
    {
        string orbType = DetermineOrbType();
        GameObject experienceOrb = ObjectPoolManager.Instance.SpawnFromPool(orbType, position, Quaternion.identity);

        if (experienceOrb != null)
        {
            ExperienceOrb orbScript = experienceOrb.GetComponent<ExperienceOrb>();
            if (orbScript != null)
            {
                orbScript.experienceAmount = orbType == "ExperienceOrb_Basic" ? 10 :
                    orbType == "ExperienceOrb_Advanced" ? 20 : 30;
                orbScript.orbType = orbType;
            }
        }
        else
        {
            Debug.LogWarning($"{orbType} 생성에 실패했습니다.");
        }
    }

    public void DropAbilityItem(Vector3 position)
    {
        Instantiate(abilityItemPrefab, position, Quaternion.identity);
    }

    // 확률에 따라 단일 아이템 드랍
    public void DropExperienceOrItem(Vector3 position)
    {
        float randomValue = Random.value;

        if (randomValue <= magnetDropRate)
        {
            // 자석 아이템 드랍
            Instantiate(magnetItemPrefab, position, Quaternion.identity);
        }
        else if (randomValue <= healthDropRate + magnetDropRate)
        {
            // 회복 아이템 드랍
            Instantiate(healthItemPrefab, position, Quaternion.identity);
        }
        else
        {
            // 확률에 해당하지 않으면 경험치 오브 드랍
            DropExperienceOrb(position);
        }
    }

    private string DetermineOrbType()
    {
        float currentTime = Time.timeSinceLevelLoad;
        if (currentTime >= 840)
        {
            float randomValue = Random.value;
            if (randomValue <= 0.1f) return "ExperienceOrb_Premium";
            else if (randomValue <= 0.4f) return "ExperienceOrb_Advanced";
        }
        else if (currentTime >= 420)
        {
            if (Random.value <= 0.3f) return "ExperienceOrb_Advanced";
        }

        return "ExperienceOrb_Basic";
    }
}
