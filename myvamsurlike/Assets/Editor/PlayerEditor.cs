using UnityEditor;
using UnityEngine;

// 추상 클래스라 SerializeField 를 사용할 수 없는 BasicWeapon 을 인스펙터에서 드롭다운 메뉴로 설정할 수 있게 만드는 커스텀 에디터
[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 Inspector 그리기
        DrawDefaultInspector();

        // Player 스크립트 대상 가져오기
        Player player = (Player)target;

        // 무기 리스트에서 currentBW 선택하는 드롭다운
        if (player.BWList != null && player.BWList.Count > 0)
        {
            string[] weaponNames = new string[player.BWList.Count];
            int currentIndex = 0;

            // BWList의 무기 이름 가져오기
            for (int i = 0; i < player.BWList.Count; i++)
            {
                weaponNames[i] = player.BWList[i].name;

                // 현재 선택된 무기 찾기
                if (player.currentBW == player.BWList[i])
                {
                    currentIndex = i;
                }
            }

            // 드롭다운을 통해 currentBW 선택
            int selectedIndex = EditorGUILayout.Popup("Current BW", currentIndex, weaponNames);

            // 선택된 무기를 currentBW로 설정
            player.currentBW = player.BWList[selectedIndex];
        }
        else
        {
            EditorGUILayout.LabelField("BWList is empty or null.");
        }

        // 변경 사항이 있을 경우 수정 사항 저장
        if (GUI.changed)
        {
            EditorUtility.SetDirty(player);
        }
    }
}