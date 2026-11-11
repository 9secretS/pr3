using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GlobalPauseMenu : MonoBehaviour
{
    public static GlobalPauseMenu Instance;

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TMP_Text jumpCounterText;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    void Awake()
    {
        Instance = this;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        isPaused = false;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void UpdateJumpCounter(int value)
    {
        if (jumpCounterText != null)
            jumpCounterText.text = "Прыжки: " + value;
    }

    public void PauseGame()
    {
        isPaused = true;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        Time.timeScale = 0f;

        PlayerControllerInspector player = FindFirstObjectByType<PlayerControllerInspector>();
        if (player != null)
            player.SetUIMode();
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ResumeGame()
    {
        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;

        PlayerControllerInspector player = FindFirstObjectByType<PlayerControllerInspector>();
        if (player != null)
            player.SetGameplayMode();
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void LoadMainMenu()
    {
        isPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(mainMenuSceneName);
    }
}