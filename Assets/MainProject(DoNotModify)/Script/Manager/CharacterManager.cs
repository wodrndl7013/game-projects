using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : Singleton<CharacterManager>
{
    public GameObject selectedCharacterPrefab; // 선택된 캐릭터 프리팹

    // 캐릭터 선택 메서드
    public void SelectCharacter(GameObject characterPrefab)
    {
        selectedCharacterPrefab = characterPrefab;
    }
    
    // 캐릭터가 선택되었는지 확인
    public bool HasSelectedCharacter()
    {
        return selectedCharacterPrefab != null;
    }
}
