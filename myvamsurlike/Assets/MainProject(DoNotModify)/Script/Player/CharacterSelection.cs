using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    public GameObject characterPrefab; // 이 버튼이 선택할 캐릭터 프리팹

    public void OnSelectCharacter()
    {
        // 선택된 캐릭터를 CharacterSelectionManager에 설정
        if (CharacterManager.Instance != null)
        {
            CharacterManager.Instance.SelectCharacter(characterPrefab);
            Debug.Log($"{characterPrefab.name} 캐릭터가 선택되었습니다.");
        }
    }
}
