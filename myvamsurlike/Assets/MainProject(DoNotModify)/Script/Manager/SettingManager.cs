using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public Slider sfxSlider; // 효과음 슬라이더
    public Slider bgmSlider; // 배경음악 슬라이더
    public GameObject settingsPanel; // 환경설정 패널

    private void Start()
    {
        // SoundManager와 슬라이더 연결
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.BindSliders(sfxSlider, bgmSlider);
            InitializeSliders(); // 초기 슬라이더 값 동기화
        }
        else
        {
            Debug.LogWarning("SoundManager.Instance가 초기화되지 않았습니다!");
        }
    }

    private void InitializeSliders()
    {
        // 슬라이더 초기값 설정 (AudioMixer의 현재 값 동기화)
        float sfxVolume, bgmVolume;

        if (SoundManager.Instance.audioMixer != null)
        {
            SoundManager.Instance.audioMixer.GetFloat(SoundManager.Instance.sfxVolumeParam, out sfxVolume);
            SoundManager.Instance.audioMixer.GetFloat(SoundManager.Instance.bgmVolumeParam, out bgmVolume);

            // 데시벨 값을 Linear 값(0~1)으로 변환
            sfxSlider.value = Mathf.Pow(10, sfxVolume / 20f);
            bgmSlider.value = Mathf.Pow(10, bgmVolume / 20f);
        }
    }

    // 환경설정 패널 닫기
    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }
}