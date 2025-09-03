using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "SO/SoundData")]
public class SoundData : ScriptableObject
{
    public string key;         // 사운드의 고유 키값
    public AudioClip audioClip; // 실제 사운드 파일
}
