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
    [Tooltip("���� �׶���Ʈ ����(0~1)")]
    [Range(0f, 1f)] public float borderGradientStrength = 0.3f;
    [Tooltip("���� ���� ���")]
    public bool useShine = true;

    [Header("Effects")]
    public GameObject highFxPrefab;  // Epic/Legendary ���� ��ƼŬ(��� OK)
    [Range(0f, 1f)] public float outerGlowAlpha = 0.12f;

    [Header("Audio")]
    public AudioClip appearSfx;      // ī�� ����
    public AudioClip rareSfx;        // ���� �̻� ����
    public AudioClip closeSfx;       // �ݱ�
}

