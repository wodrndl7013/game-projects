using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    // UI 패널 연결
    public GameObject settingsPanel;   // 설정 패널 (ESC 키로 활성화/비활성화)
    public GameObject gameOverPanel;   // 게임 오버 패널
    public GameObject pauseMenu;       // 일시정지 메뉴 패널

    // 게임 상태 관리 플래그
    private bool isGameOver = false;
    private bool isPaused = false;

    private void Start()
    {
        // 게임 시작 시 모든 패널 비활성화
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // ESC 키 입력 시 일시정지/재개 토글, 게임 오버 상태가 아닐 때만 작동
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    // 게임 시작 버튼 클릭 시 호출될 메서드
    public void StartGame()
    {
        SceneManager.LoadScene("StageSelect"); // StageSelect 씬으로 이동
    }

    // 게임 종료 버튼 클릭 시 호출될 메서드
    public void ExitGame()
    {
        Application.Quit(); // 게임 종료
    }

    // 게임 오버 상태 전환
    public void GameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        Time.timeScale = 0f; // 게임 정지
        isGameOver = true;   // 게임 오버 플래그 설정
    }
    
    public void RestartGame()
    {
        // 게임 초기화를 위해 게임 오버 및 일시정지 상태 해제
        isGameOver = false;
        isPaused = false;

        // 게임 오버 패널과 일시정지 패널 비활성화
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }

        // 게임 속도 초기화
        Time.timeScale = 1f;

        // 현재 씬 다시 로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 일시정지 상태 전환
    public void PauseGame()
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }
        Time.timeScale = 0f; // 게임 정지
        isPaused = true;     // 일시정지 플래그 설정
    }

    // 게임 재개
    public void ResumeGame()
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        Time.timeScale = 1f; // 게임 속도 정상화
        isPaused = false;    // 일시정지 해제
    }

    // 환경설정 메뉴 열기
    public void OpenSettings()
    {
        if (settingsPanel != null && !isGameOver)
        {
            settingsPanel.SetActive(true);
        }
    }

    // 환경설정 메뉴 닫기
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    // 특정 스테이지 씬으로 이동
    public void LoadStage(string stageName)
    {
        // Game Over 또는 일시정지 상태에서 호출되지 않도록 설정
        if (!isGameOver)
        {
            Time.timeScale = 1f; // 게임 속도 정상화
            SceneManager.LoadScene(stageName);
        }
    }

    // 스테이지 선택 화면으로 이동
    public void ExitToStageSelect()
    {
        Time.timeScale = 1f; // 게임 속도 정상화
        SceneManager.LoadScene("StageSelect");
    }

    // Game Over 상태 확인 메서드 (다른 스크립트에서 호출 가능)
    public bool IsGameOver()
    {
        return isGameOver;
    }

    // 일시정지 상태 확인 메서드 (다른 스크립트에서 호출 가능)
    public bool IsPaused()
    {
        return isPaused;
    }
}
