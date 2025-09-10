using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CatchU/Rarity Style")]
public class RarityStyle : ScriptableObject
{
    [Header("Visuals")]
    public Color nameColor = Color.white;
    public Color rarityColor = Color.white;
    public Color borderColor = Color.white;
    [Tooltip("보더 그라디언트 세기(0~1)")]
    [Range(0f, 1f)] public float borderGradientStrength = 0.3f;
    [Tooltip("샤인 스윕 사용")]
    public bool useShine = true;

    [Header("Effects")]
    public GameObject highFxPrefab;  // Epic/Legendary 전용 파티클(없어도 OK)
    [Range(0f, 1f)] public float outerGlowAlpha = 0.12f;

    [Header("Audio")]
    public AudioClip appearSfx;      // 카드 등장
    public AudioClip rareSfx;        // 레어 이상 강조
    public AudioClip closeSfx;       // 닫기
}

