using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string firstGameSceneName = "GameScene";

    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(firstGameSceneName);
    }

    public void ExitGame()
    {
        UnityEngine.Application.Quit();
        UnityEngine.Debug.Log("Игра закрыта");
    }
}