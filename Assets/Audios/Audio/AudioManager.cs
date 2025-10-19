using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    // Singleton pattern: Bu script'ten sadece bir tane olmas�n� sa�lar.
    public static AudioManager instance;

    [Header("Audio Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip gameSceneMusic;
    public AudioClip miniGameSceneMusic;

    private AudioSource audioSource;
    private const string VOLUME_KEY = "MusicVolume"; // PlayerPrefs i�in anahtar

    void Awake()
    {
        // Singleton'� kur
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Bu objenin sahne ge�i�lerinde yok olmamas�n� sa�la
        }
        else
        {
            Destroy(gameObject); // E�er zaten bir AudioManager varsa, bu yenisini yok et
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // Kay�tl� ses seviyesini y�kle ve uygula
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 0.75f); // E�er kay�tl� veri yoksa varsay�lan 0.75 olsun
        SetMusicVolume(savedVolume);

        // SceneManager event'ine abone ol
        SceneManager.sceneLoaded += OnSceneLoaded;

        // �lk sahnenin m�zi�ini manuel olarak �al (��nk� OnSceneLoaded bu sahne i�in tetiklenmeyebilir)
        PlayMusicForScene(SceneManager.GetActiveScene());
    }

    // Bir sahne y�klendi�inde bu fonksiyon �a�r�l�r
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene);
    }

    private void PlayMusicForScene(Scene scene)
    {
        AudioClip clipToPlay = null;

        // Sahne ad�na g�re do�ru m�zi�i se�
        if (scene.name == "MainMenu") // SEN�N ANA MEN� SAHNEN�N ADI NEYSE ONU YAZ
        {
            clipToPlay = mainMenuMusic;
        }
        else if (scene.name == "Map") // SEN�N OYUN SAHNEN�N ADI NEYSE ONU YAZ
        {
            clipToPlay = gameSceneMusic;
        }


        if (clipToPlay != null && audioSource.clip != clipToPlay)
        {
            audioSource.clip = clipToPlay;
            audioSource.Play();
        }
    }

    public void SetMusicVolume(float volume)
    {
        audioSource.volume = volume;
        PlayerPrefs.SetFloat(VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}