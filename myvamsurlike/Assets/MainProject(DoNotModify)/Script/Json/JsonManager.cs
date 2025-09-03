using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class JsonManager : Singleton<JsonManager>
{
    public TextAsset jsonFile; // 텍스트 데이터를 가진 JSON 파일
    private ItemSheet itemData; // JSON 데이터를 들고 있을 리스트
    private Dictionary<string, Dictionary<int, string>> levelUpText; // 실제 값이 저장된 딕셔너리
    private Dictionary<string, string> itemNameDic;

    void Awake()
    {
        LoadJson();
    }
    
    private void LoadJson() // json데이터 딕셔너리에 파싱
    {
        if (jsonFile == null)
        {
            Debug.LogError("JSON 파일이 지정되지 않았습니다.");
            return;
        }
        
        itemData = JsonUtility.FromJson<ItemSheet>(jsonFile.text);

        levelUpText = new Dictionary<string, Dictionary<int, string>>();
        itemNameDic = new Dictionary<string, string>();

        for (int i = 0; i < itemData.itemSheet.Count; i++)
        {
            string itemID = itemData.itemSheet[i].itemID;
            levelUpText[itemID] = new Dictionary<int, string>
            {
                { 0, itemData.itemSheet[i].level0 },
                { 1, itemData.itemSheet[i].level1 },
                { 2, itemData.itemSheet[i].level2 },
                { 3, itemData.itemSheet[i].level3 },
                { 4, itemData.itemSheet[i].level4 },
                { 5, itemData.itemSheet[i].level5 },
                { 6, itemData.itemSheet[i].level6 }
            };

            itemNameDic[itemID] = itemData.itemSheet[i].itemName;
        }
    }

    public string PrintLeveUpText(string itemID) // 무기의 현재 레벨에 맞는 문구 가져옴
    {
        Dictionary<int, string> dic = levelUpText[itemID];
        int level = WeaponManager.Instance.GetLevel(itemID);
        return dic[level];
    }

    public string PrintItemName(string itemID)
    {
        return itemNameDic[itemID];
    }
    
    [System.Serializable]
    public class Item
    {
        public string itemID;
        public string itemName;
        public string level0;
        public string level1;
        public string level2;
        public string level3;
        public string level4;
        public string level5;
        public string level6;
    }

    [System.Serializable]
    public class ItemSheet
    {
        public List<Item> itemSheet;
    }
}

