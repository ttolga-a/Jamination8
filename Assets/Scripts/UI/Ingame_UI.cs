using TMPro;
using UnityEngine;

public class Ingame_UI : MonoBehaviour
{
    public static Ingame_UI instance;

    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject pauseUI;
    public TMP_Text bombUnstableText;
    private bool isPaused = false;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        ShowTimerOnScreen();
        PauseButton();
    }

    private void ShowTimerOnScreen()
    {
        float remainingTime = BombManager.instance.bombRemainingTime;

        if (remainingTime < 0)
        {
            remainingTime = 0;
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);

        timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }

    private void PauseButton()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                isPaused = false;
                Time.timeScale = 1;
                pauseUI.SetActive(false);
            }
            else
            {
                isPaused = true;
                Time.timeScale = 0;
                pauseUI.SetActive(true);
            }
        }
    }

    public void ResumeButton()
    {
        pauseUI.SetActive(!pauseUI.activeSelf);
        Time.timeScale = 1;
    }
}