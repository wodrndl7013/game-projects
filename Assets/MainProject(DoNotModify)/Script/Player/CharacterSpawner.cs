using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public Vector3 spawnPosition = new Vector3(0, 0, 0); // 캐릭터 스폰 위치

    public void Start()
    {
        if (CharacterManager.Instance != null && CharacterManager.Instance.selectedCharacterPrefab != null)
        {
            // 선택한 캐릭터 프리팹을 스폰 위치에 생성
            Instantiate(CharacterManager.Instance.selectedCharacterPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("선택된 캐릭터가 없습니다.");
        }
    }
}
