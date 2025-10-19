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
        // Singleton koruma
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 🧠 Kaydedilmiş ilerlemeyi yükle
        LoadProgress();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Oyuncu harita sahnesine dönünce konumunu geri yükle
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

    // ✅ Bilgileri kalıcı hale getir
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
