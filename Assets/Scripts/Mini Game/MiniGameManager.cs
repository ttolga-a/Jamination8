using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance;

    [Header("UI Elemanları")]
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text resultText;
    public GameObject resultPanel;
    public Button actionButton;
    public TMP_Text actionButtonText;
    public Button startButton;

    [Header("Ayarlar")]
    public float gameDuration = 20f;
    public int targetScore = 15;

    [HideInInspector] public int currentScore = 0;
    [HideInInspector] public bool gameActive = false;

    private float currentTime;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameActive = false;                  // oyun başta duracak
        actionButton.gameObject.SetActive(false);
        startButton.gameObject.SetActive(true); // başla butonu aktif
    }

    private void Update()
    {
        if (!gameActive) return;

        currentTime -= Time.deltaTime;
        timerText.text = $"Süre: {Mathf.CeilToInt(currentTime)}";

        if (currentTime <= 0f)
        {
            EndGame();
        }
    }

    public void StartGame()
    {
        gameActive = true;
        startButton.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);
        currentScore = 0;
        currentTime = gameDuration;
        scoreText.text = $"Puan: {currentScore}";
        timerText.text = $"Süre: {Mathf.CeilToInt(currentTime)}";
    }

    public void AddScore()
    {
        if (!gameActive) return;

        currentScore++;
        scoreText.text = $"Puan: {currentScore}";
    }

    private void EndGame()
    {
        gameActive = false;
        resultPanel.SetActive(true);
        actionButton.gameObject.SetActive(true);

        if (currentScore >= targetScore) // kazanma
        {
            resultText.text = $"Tebrikler! Puan: {currentScore}";
            resultText.color = Color.green;
            actionButtonText.text = "Devam Et";

            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() => LoadNextScene("GameScene"));
        }
        else // kaybetme
        {
            resultText.text = $"Kaybettin! Puan: {currentScore}";
            resultText.color = Color.red;
            actionButtonText.text = "Tekrar Dene";

            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });
        }
    }

    private void LoadNextScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
