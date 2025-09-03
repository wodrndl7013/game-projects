using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu; // PauseMenu 패널

    private bool isPaused = false;

    void Update()
    {
        // Esc 키 입력으로 게임 일시정지/재개 전환
        if (Input.GetKeyDown(KeyCode.Escape))
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

    // 게임 일시정지
    public void PauseGame()
    {
        pauseMenu.SetActive(true); // 일시정지 메뉴 활성화
        Time.timeScale = 0f; // 게임 시간 멈춤
        isPaused = true;
    }
    
    // 게임 재개 함수
    public void ResumeGame()
    {
        pauseMenu.SetActive(false); // 일시정지 메뉴 비활성화
        Time.timeScale = 1f; // 게임 시간 재개
        isPaused = false;
    }
    
    // 스테이지 선택 화면으로 이동하는 함수
    public void ExitToStageSelect()
    {
        Time.timeScale = 1f; // 게임 시간을 다시 원래대로 설정
        SceneManager.LoadScene("StageSelect"); // "StageSelect"라는 씬으로 이동
    }
}
