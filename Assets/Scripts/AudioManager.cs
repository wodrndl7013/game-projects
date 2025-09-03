using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource bgmSource;

    private void Awake()
    {
        // AudioManager 싱글톤 패턴 설정
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Scene 전환 시에도 유지되도록 설정
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Scene 로드 시 이벤트 등록
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Scene에 따라 BGM 설정
        if (scene.name == "StartScene" || scene.name == "StageSelectScene")
        {
            // StartScene이나 StageSelectScene인 경우
            if (!bgmSource.isPlaying)
            {
                bgmSource.Play(); // BGM 재생
            }
        }
        else
        {
            // 다른 Scene(StageScene 등)인 경우
            if (bgmSource.isPlaying)
            {
                bgmSource.Stop(); // BGM 정지
            }
        }
    }
}