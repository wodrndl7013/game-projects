using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSeclectController : MonoBehaviour
{
    public void OnStage1ButtonClicked()
    {
        SceneManager.LoadScene("Stage1Scene");
    }

    public void OnStage2ButtonClicked()
    {
        SceneManager.LoadScene("Stage2Scene");
    }

    public void OnStage3ButtonClicked()
    {
        SceneManager.LoadScene("Stage3Scene");
    }
    
    public void OnStage4ButtonClicked()
    {
        SceneManager.LoadScene("Stage4Scene");
    }
    
    public void OnStage5ButtonClicked()
    {
        SceneManager.LoadScene("Stage5Scene");
    }
    // 필요한 만큼의 메서드를 추가합니다.
}
