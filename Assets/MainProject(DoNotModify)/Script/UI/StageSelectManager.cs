using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    public void LoadStage(string stageName)
    {
        SceneManager.LoadScene("MainScene Lee");
    }
}