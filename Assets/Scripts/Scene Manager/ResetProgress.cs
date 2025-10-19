using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetProgress : MonoBehaviour
{
    [Header("Tuş Ayarı")]
    [SerializeField] private KeyCode resetKey = KeyCode.R; // 🔁 İstersen başka tuş atayabilirsin (örneğin KeyCode.T)

    private void Update()
    {
        if (Input.GetKeyDown(resetKey))
        {
            // 🔄 Tüm PlayerPrefs verisini sil
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            // SceneDataManager varsa anlık belleği de sıfırla
            if (SceneDataManager.Instance != null)
            {
                SceneDataManager.Instance.wiseMonkeyDone = false;
                SceneDataManager.Instance.playerPosition = Vector3.zero;
                SceneDataManager.Instance.lastSceneName = "";
            }

            Debug.Log("🔄 Tüm ilerleme sıfırlandı! (WiseMonkeyDone = false)");

            // İsteğe bağlı: şu anki sahneyi yeniden yükle
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
