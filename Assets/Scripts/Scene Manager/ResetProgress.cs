using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetProgress : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Editor Başlatıldığında Sıfırlama")]
    [Tooltip("Oyun her editörden başlatıldığında PlayerPrefs temizlenir.")]
    [SerializeField] private bool autoResetOnPlay = true;
#endif

    private void Awake()
    {
#if UNITY_EDITOR
        // 🎮 Editörden başlatıldığında otomatik sıfırlama
        if (autoResetOnPlay)
        {
            ResetAllProgress();
        }
#endif
    }

    private void ResetAllProgress()
    {
        // 🔄 PlayerPrefs temizle
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // 🔄 SceneDataManager varsa runtime belleğini de sıfırla
        if (SceneDataManager.Instance != null)
        {
            SceneDataManager.Instance.wiseMonkeyDone = false;
            SceneDataManager.Instance.playerPosition = Vector3.zero;
            SceneDataManager.Instance.lastSceneName = "";
        }

        Debug.Log("🧹 Oyun verisi sıfırlandı! (Editor veya manuel reset)");

        // 🔁 Sahneyi yenile
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
