using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

// 플레이어가 들 수 있는 아이템 리스트와 정보를 다룸
// 플레이어의 자식으로 들어가고, 아이템들을 자식으로 들고 있으면서 해당 프리팹들을 딕셔너리에서 String 으로 매핑
// 레벨업 보상에서 매니저 함수를 호출하여 아이템 전반을 업그레이드 하는 시스템
public class WeaponManager : Singleton<WeaponManager>
{
    public GameObject[] Items; // 참조할 아이템 프리팹
    public ItemData[] ItemDatas; // 참조할 아이템 데이터
    private Dictionary<string, int> itemLeveles = new Dictionary<string, int>(); // 아이템 레벨 다뤄줄 Dic
    private Dictionary<string, (ItemData, GameObject)> ItemDic = new Dictionary<string, (ItemData, GameObject)>(); // 프리팹과 데이터를 연결시켜주는 Dic
    public Player _player;
    
    private float angelProb = 0.5f; // 초기 확률 50%
    private float demonProb = 0.5f;

    // Gear를 위한 타입별 활성화된 무기 리스트
    private List<Weapon> AngelWeapons = new();
    private List<Weapon> DemonWeapons = new();
    
    void Awake()
    {
        Setting();
    }
    
    void Setting()
    {
        if (ItemDatas == null)
        {
            Debug.LogError("ItemDatas 배열이 null입니다. 인스펙터에서 할당되었는지 확인하세요.");
            return;
        }
        
        // 복제한 ItemData와 Item을 연결하여 딕셔너리에 저장 !!! ItemData의 itemType 과 Item 프리팹의 이름이 일치하여야 함
        foreach (var itemData in ItemDatas)
        {
            foreach (var weapon in Items)
            {
                if (weapon.name == itemData.itemName)
                {
                    ItemDic[itemData.itemName] = (itemData, weapon);
                    weapon.SetActive(false);
                    break;
                }
            }
        }

        // ItemData 들의 이름을 키값으로 레벨 딕셔너리 추가
        foreach (var itemData in ItemDatas)
        {
            itemLeveles[itemData.itemName] = 0; // 초기 레벨 0으로 설정
        }

        // 플레이어 받아옴
        _player = GetComponentInParent<Player>();
    }

    private (ItemData, GameObject)? GetDicValue(string itemType) // 딕셔너리에 저장된 itemData 와 아이템의 프리팹을 꺼내옴
    {
        ItemDic.TryGetValue(itemType, out var itemInfo);
        return itemInfo;
    }

    private Component GetUnknownComponent(GameObject item) // 레벨업 할 아이템이 장비 or 기어인지 판단
    {
        var component = item.GetComponent<Weapon>() as Component;
        if (component == null)
        {
            component = item.GetComponent<Gear>() as Component;
        }

        return component;
    }

    private int GetItemLevelUp(string itemType) // 아이템 레벨 딕셔너리에서 특정 아이템 레벨업
    {
        itemLeveles[itemType]++;
        return itemLeveles[itemType];
    }

    private float CalculateDamage(ItemData itemData, int level) // 데미지 계산식
    {
        if (itemData.itemType == ItemData.ItemType.Angel)
        {
            if (ItemDic["DMGBuff_Angel"].Item2.activeSelf)
            { 
                int dmgBuffLevel = GetLevel("DMGBuff_Angel"); 
                return (itemData.baseDamage + (itemData.baseDamage * itemData.damages[level - 1])) * ItemDic["DMGBuff_Angel"].Item1.damages[dmgBuffLevel - 1];
            }
            else
            {
                return itemData.baseDamage + (itemData.baseDamage * itemData.damages[level - 1]);
            }
        }
        else if (itemData.itemType == ItemData.ItemType.Demon)
        {
            if (ItemDic["DMGBuff_Demon"].Item2.activeSelf)
            { 
                int dmgBuffLevel = GetLevel("DMGBuff_Demon"); 
                return (itemData.baseDamage + (itemData.baseDamage * itemData.damages[level - 1])) * ItemDic["DMGBuff_Demon"].Item1.damages[dmgBuffLevel - 1];
            }
            else
            {
                return itemData.baseDamage + (itemData.baseDamage * itemData.damages[level - 1]);
            }
        }

        return 0f;
    }

    private void ResistActiveList(Weapon weapon, ItemData itemData)
    {
        if (itemData.itemType == ItemData.ItemType.Angel)
        {
            AngelWeapons.Add(weapon);
        }
        else if (itemData.itemType == ItemData.ItemType.Demon)
        {
            DemonWeapons.Add(weapon);
        }
    }
    
    public void LevelUpdate(string itemType) // 외부에서 사용할 아이템 레벨업 시 State 업데이트 해주는 메서드
    {
        var itemInfo = GetDicValue(itemType);

        if (itemInfo.HasValue && !itemInfo.Value.Item2.activeSelf) // 아이템이 비활성화 된 경우(레벨 0 일 경우)
            itemInfo.Value.Item2.SetActive(true);
        
        // Weapon, Gear 타입에 나누어 Data 업데이트. 레벨업이 특정 무기 전역에 영향을 미치는 Gear 의 경우 브로드캐스트를 통해 무기들에게 해당 정보가 변형되었음을 알림
        if (itemInfo != null)
        {
            ItemData itemData = itemInfo.Value.Item1;
            GameObject item = itemInfo.Value.Item2;
            int level = GetItemLevelUp(itemType);
        
            // 아이템의 타입에 따라 처리
            var itemComponent = GetUnknownComponent(item);
            if (itemComponent is Weapon weapon)
            {
                // !! 기어 추가시 로직 변경 필요
                weapon.name = itemData.itemName;
                weapon.damage = CalculateDamage(itemData, level);
                weapon.count = itemData.baseCount + itemData.counts[level - 1];
                weapon.speed = itemData.speed;
                weapon.attackDistance = itemData.distance;
                weapon.cooldown = itemData.cooldown;
                weapon.duration = itemData.duration;
                weapon.LevelUpSetting(); // !! 한프레임에 작동 안할 경우 오류 일어날 수 있음을 생각 = 작동 하는거 같음

                if (level == 1) // 무기 장착 순간이라면
                {
                    ResistActiveList(weapon, itemData);
                }
            }
            else if (itemComponent is Gear gear)
            {
                gear.rate = itemData.damages[level - 1];
                gear.UpgradeBroadcast();
            }
            else
            {
                Debug.LogWarning("아이템이 Weapon 또는 Gear 타입이 아닙니다.");
            }

            // if (level > 6) // !!! 여기에 7레벨 된 무기들 리스트에 추가하는 로직 짜고, 합성 가능한 아이템이 있는지 확인하는 로직 짜기
            // {
            //     ItemDic.Remove(itemType); 
            // }
        }
    }

    public int GetLevel(string itemName)
    {
        return itemLeveles[itemName];
    }
    
    public List<ItemData> GetRandomItemDataList(int count) // 확률에 기반하여 랜덤 아이템 값을 반환
    {
        List<ItemData> selectedItems = new List<ItemData>();
        System.Random rng = new System.Random();

        // 조건에 맞는 아이템만 필터링 (레벨이 7 미만인 아이템만 포함)
        List<ItemData> availableAngelItems = new List<ItemData>(
            GetItemsByType(ItemData.ItemType.Angel).FindAll(item => itemLeveles[item.itemName] < 7)
        );

        List<ItemData> availableDemonItems = new List<ItemData>(
            GetItemsByType(ItemData.ItemType.Demon).FindAll(item => itemLeveles[item.itemName] < 7)
        );

        while (selectedItems.Count < count)
        {
            float randomValue = (float)rng.NextDouble();
            List<ItemData> possibleItems;

            if (randomValue < angelProb && availableAngelItems.Count > 0)
            {
                possibleItems = availableAngelItems;
            }
            else if (availableDemonItems.Count > 0)
            {
                possibleItems = availableDemonItems;
            }
            else
            {
                // 한쪽의 리스트가 비었을 때 다른 리스트에서 가져옴
                possibleItems = availableAngelItems.Count > 0 ? availableAngelItems : availableDemonItems;
            }

            if (possibleItems.Count > 0)
            {
                ItemData selectedItem = possibleItems[rng.Next(possibleItems.Count)];
                selectedItems.Add(selectedItem);
                possibleItems.Remove(selectedItem); // 중복 방지를 위해 선택된 아이템 제거
            }
            else
            {
                break; // 무한 루프 방지
            }
        }

        return selectedItems;
    }

    private List<ItemData> GetItemsByType(ItemData.ItemType type) 
    {
        List<ItemData> items = new List<ItemData>();
        foreach (var item in ItemDic.Values)
        {
            if (item.Item1.itemType == type)
                items.Add(item.Item1);
        }
        return items;
    }
    
    public void AdjustProbabilities(ItemData.ItemType selectedType) // 선택된 타입에 따라 확률 조정
    {
        float adjustmentFactor = 0.05f; // 확률 조정 정도 
        float minProb = 0.1f; // 최소 확률

        if (selectedType == ItemData.ItemType.Angel)
        {
            angelProb = Mathf.Clamp(angelProb + adjustmentFactor, minProb, 1 - minProb);
            demonProb = 1f - angelProb;
        }
        else if (selectedType == ItemData.ItemType.Demon)
        {
            demonProb = Mathf.Clamp(demonProb + adjustmentFactor, minProb, 1 - minProb);
            angelProb = 1f - demonProb;
        }

        EclipseManager.Instance.CheckingEclipse(angelProb);
    }
    
    // Gear BroadCast 로직들
    public void Broadcast_DMGBUff_Angel(float rate)
    {
        foreach (var weapon in AngelWeapons)
        {
            ItemData itemData = ItemDic[weapon.name].Item1;
            int level = GetLevel(weapon.name);
            weapon.damage = (itemData.baseDamage + (itemData.baseDamage * itemData.damages[level - 1])) * rate;
            weapon.LevelUpSetting();
        }
    }
    
    public void Broadcast_DMGBUff_Demon(float rate)
    {
        foreach (var weapon in DemonWeapons)
        {
            ItemData itemData = ItemDic[weapon.name].Item1;
            int level = GetLevel(weapon.name);
            weapon.damage = (itemData.baseDamage + (itemData.baseDamage * itemData.damages[level - 1])) * rate;
            weapon.LevelUpSetting();
        }
    }
    
    // 나중에 무기 합성해서 사라질 때 모든 리스트 및 딕셔너리에서 제거하는 로직도 짜야할듯
}
