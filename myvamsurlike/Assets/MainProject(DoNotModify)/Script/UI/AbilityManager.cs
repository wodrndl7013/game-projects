using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class AbilityManager : Singleton<AbilityManager>
{
    public GameObject abilitySelectionPanel; // 능력 선택 패널
    public Transform abilityCardContainer; // 능력 카드들을 담는 컨테이너
    public GameObject abilityCardPrefab; // 능력 카드 프리팹

    private Player player; // 플레이어 참조 (ExperienceManager에서 설정)

    private void Start()
    {
        abilitySelectionPanel.SetActive(false);
    }

    public void Initialize(Player player)
    {
        this.player = player;
    }

    public void ShowAbilitySelection()
    {
        //게임을 멈추기 위해 Time.timescale을 0으로 설정
        Time.timeScale = 0f;
        
        abilitySelectionPanel.SetActive(true);

        // 기존 카드가 있다면 삭제
        foreach (Transform child in abilityCardContainer)
        {
            Destroy(child.gameObject);
        }

        // 무작위로 능력 3개를 선택해 카드 생성
        List<ItemData> randomAbilities = WeaponManager.Instance.GetRandomItemDataList(3);
        foreach (ItemData ability in randomAbilities)
        {
            GameObject card = Instantiate(abilityCardPrefab, abilityCardContainer);
            // 카드 UI 설정
            card.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = JsonManager.Instance.PrintItemName(ability.itemName);
            card.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>().text = JsonManager.Instance.PrintLeveUpText(ability.itemName);
            card.transform.Find("Icon").GetComponent<Image>().sprite = ability.itemIcon;

            Button selectButton = card.transform.Find("SelectButton").GetComponent<Button>();
            selectButton.onClick.AddListener(() => SelectAbility(ability));
        }
    }

    void SelectAbility(ItemData ability)
    {
        // 능력을 플레이어에게 적용
        WeaponManager.Instance.LevelUpdate(ability.itemName);
        
        // 선택된 능력에 따라 앞으로 있을 레벨업 보상에서 등장할 아이템 타입 확률 조정
        WeaponManager.Instance.AdjustProbabilities(ability.itemType);

        // 패널 비활성화
        abilitySelectionPanel.SetActive(false);
        
        // 게임을 다시 재개하기 위해 Time.timescale을 1로 설정
        Time.timeScale = 1f;
    }
}