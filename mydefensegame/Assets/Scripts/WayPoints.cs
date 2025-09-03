using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoints : MonoBehaviour // WayPoints 클래스 정의, MonoBehaviour를 상속받음
{
    public static Transform[] points; // 정적 Transform 배열 선언, 웨이포인트를 저장할 예정

    void Awake() // Awake 메서드 : 스크립트가 로드될 때 실행됨
    {
        points = new Transform[transform.childCount]; // points 배열을 현재 오브젝트의 자식 개수만큼의 크기로 초기화
        for (int i = 0; i < points.Length; i++) // 모든 자식 오브젝트를 순회하며 points 배열에 저장
        {
            points[i] = transform.GetChild(i);
        }
    }
}
