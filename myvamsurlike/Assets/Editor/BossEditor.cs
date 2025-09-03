using UnityEditor;
using UnityEngine;


// 추상 클래스라 SerializeField 를 사용할 수 없는 BasicWeapon 을 인스펙터에서 드롭다운 메뉴로 설정할 수 있게 만드는 커스텀 에디터
[CustomEditor(typeof(Boss))]
public class BossEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 Inspector 그리기
        DrawDefaultInspector();

        // Boss 스크립트 대상 가져오기
        Boss boss = (Boss)target;

        // 무기 리스트에서 currentBS 선택하는 드롭다운
        if (boss.BSList != null && boss.BSList.Count > 0)
        {
            string[] weaponNames = new string[boss.BSList.Count];
            int currentIndex = 0;

            // BSList의 무기 이름 가져오기
            for (int i = 0; i < boss.BSList.Count; i++)
            {
                weaponNames[i] = boss.BSList[i].name;

                // 현재 선택된 무기 찾기
                if (boss.currentBS == boss.BSList[i])
                {
                    currentIndex = i;
                }
            }

            // 드롭다운을 통해 currentBS 선택
            int selectedIndex = EditorGUILayout.Popup("Current BW", currentIndex, weaponNames);

            // 선택된 무기를 currentBS로 설정
            boss.currentBS = boss.BSList[selectedIndex];
        }
        else
        {
            EditorGUILayout.LabelField("BWList is empty or null.");
        }

        // 변경 사항이 있을 경우 수정 사항 저장
        if (GUI.changed)
        {
            EditorUtility.SetDirty(boss);
        }
    }
}