using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDataManager : MonoBehaviour
{
    public static SceneDataManager Instance;

    [Header("Kaydedilecek Veriler")]
    public Vector3 playerPosition;
    public string lastSceneName;
    public bool wiseMonkeyDone = false; // ✅ Maymun etkileşimi tamamlandı mı?

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Map")
        {
            var player = FindFirstObjectByType<Player>();
            if (player != null && playerPosition != Vector3.zero)
                player.transform.position = playerPosition;
        }
    }

    public void SavePlayerData(Vector3 position, string currentScene)
    {
        playerPosition = position;
        lastSceneName = currentScene;
    }

    // ✅ Kalıcı hale getirmek istersen (PlayerPrefs ile)
    public void SaveProgress()
    {
        PlayerPrefs.SetInt("WiseMonkeyDone", wiseMonkeyDone ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadProgress()
    {
        if (PlayerPrefs.HasKey("WiseMonkeyDone"))
            wiseMonkeyDone = PlayerPrefs.GetInt("WiseMonkeyDone") == 1;
    }
}
