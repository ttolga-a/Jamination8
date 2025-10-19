using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    // Singleton pattern: Bu script'ten sadece bir tane olmasýný saðlar.
    public static AudioManager instance;

    [Header("Audio Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip gameSceneMusic;

    private AudioSource audioSource;
    private const string VOLUME_KEY = "MusicVolume"; // PlayerPrefs için anahtar

    void Awake()
    {
        // Singleton'ý kur
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Bu objenin sahne geçiþlerinde yok olmamasýný saðla
        }
        else
        {
            Destroy(gameObject); // Eðer zaten bir AudioManager varsa, bu yenisini yok et
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // Kayýtlý ses seviyesini yükle ve uygula
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 0.75f); // Eðer kayýtlý veri yoksa varsayýlan 0.75 olsun
        SetMusicVolume(savedVolume);

        // SceneManager event'ine abone ol
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Ýlk sahnenin müziðini manuel olarak çal (çünkü OnSceneLoaded bu sahne için tetiklenmeyebilir)
        PlayMusicForScene(SceneManager.GetActiveScene());
    }

    // Bir sahne yüklendiðinde bu fonksiyon çaðrýlýr
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene);
    }

    private void PlayMusicForScene(Scene scene)
    {
        AudioClip clipToPlay = null;

        // Sahne adýna göre doðru müziði seç
        if (scene.name == "MainMenu") // SENÝN ANA MENÜ SAHNENÝN ADI NEYSE ONU YAZ
        {
            clipToPlay = mainMenuMusic;
        }
        else if (scene.name == "TolgaTestScene") // SENÝN OYUN SAHNENÝN ADI NEYSE ONU YAZ
        {
            clipToPlay = gameSceneMusic;
        }

        // Eðer çalýnan müzik zaten doðruysa tekrar baþlatma
        if (clipToPlay != null && audioSource.clip != clipToPlay)
        {
            audioSource.clip = clipToPlay;
            audioSource.Play();
        }
    }

    // Slider'dan gelen deðeri alýp sesi ayarlar ve kaydeder
    public void SetMusicVolume(float volume)
    {
        audioSource.volume = volume;
        PlayerPrefs.SetFloat(VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    // Program kapatýlýrken event'ten aboneliði kaldýr (hafýza sýzýntýsýný önler)
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}