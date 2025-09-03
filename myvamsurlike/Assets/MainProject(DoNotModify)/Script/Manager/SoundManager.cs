using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; } // 싱글톤 인스턴스

    [Header("Sound Data")]
    public List<SoundData> soundDataList; // SoundData를 직접 Inspector에 추가

    public Dictionary<string, AudioClip> audioDictionary = new Dictionary<string, AudioClip>();
    [Header("Audio Sources")]
    public AudioSource bgmSource; // 배경음악 AudioSource
    public AudioSource sfxSource; // 효과음 AudioSource

    [Header("Audio Mixer")]
    public AudioMixer audioMixer; // AudioMixer를 Inspector에 연결
    public string bgmVolumeParam = "BGMVolume"; // BGM 볼륨 변수 이름
    public string sfxVolumeParam = "SFXVolume"; // SFX 볼륨 변수 이름

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // SoundData 초기화
        InitializeSoundData();

        // 초기 볼륨 로드
        LoadVolumeSettings();
    }

    private void InitializeSoundData()
    {
        foreach (var soundData in soundDataList)
        {
            if (!audioDictionary.ContainsKey(soundData.key))
            {
                audioDictionary.Add(soundData.key, soundData.audioClip);
            }
        }
    }

    public void PlaySound(string key)
    {
        if (audioDictionary.ContainsKey(key))
        {
            sfxSource.PlayOneShot(audioDictionary[key]);
        }
        else
        {
            Debug.LogWarning($"Sound with key '{key}' not found.");
        }
    }

    public void PlayExplosionSound()
    {
        PlaySound("ExplosionSound");
    }
    
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource != null && clip != null)
        {
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void SetBGMVolume(float volume)
    {
        // Linear (0.0~1.0) -> dB 변환
        float dbVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat(sfxVolumeParam, dbVolume);
    }


    public void SetSFXVolume(float volume)
    {
        // Linear (0.0~1.0) -> dB 변환
        float dbVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat(bgmVolumeParam, dbVolume);
    }

    public void BindSliders(Slider sfxSlider, Slider bgmSlider)
    {
        if (sfxSlider != null)
        {
            // 슬라이더 초기값 설정
            float sfxVolume;
            audioMixer.GetFloat(sfxVolumeParam, out sfxVolume);
            sfxSlider.value = Mathf.Pow(10, sfxVolume / 20f); // dB -> Linear 변환

            // 슬라이더 값 변경 시 호출될 이벤트 연결
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        if (bgmSlider != null)
        {
            // 슬라이더 초기값 설정
            float bgmVolume;
            audioMixer.GetFloat(bgmVolumeParam, out bgmVolume);
            bgmSlider.value = Mathf.Pow(10, bgmVolume / 20f); // dB -> Linear 변환

            // 슬라이더 값 변경 시 호출될 이벤트 연결
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        }
    }

    public void SaveVolumeSettings()
    {
        float sfxVolume, bgmVolume;
        audioMixer.GetFloat(sfxVolumeParam, out sfxVolume);
        audioMixer.GetFloat(bgmVolumeParam, out bgmVolume);

        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        PlayerPrefs.Save();
    }

    public void LoadVolumeSettings()
    {
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
            audioMixer.SetFloat(sfxVolumeParam, sfxVolume);
        }
        if (PlayerPrefs.HasKey("BGMVolume"))
        {
            float bgmVolume = PlayerPrefs.GetFloat("BGMVolume");
            audioMixer.SetFloat(bgmVolumeParam, bgmVolume);
        }
    }
}
