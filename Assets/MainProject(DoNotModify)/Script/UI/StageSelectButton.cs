using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectButton : MonoBehaviour
{
    public string stageSceneName; // 로드할 스테이지 씬 이름

    public void OnSelectStage()
    {
        if (CharacterManager.Instance != null && CharacterManager.Instance.selectedCharacterPrefab != null)
        {
            // 스테이지 씬으로 이동
            SceneManager.LoadScene(stageSceneName);
        }
        else
        {
            Debug.LogWarning("캐릭터가 선택되지 않았습니다.");
        }
    }
}