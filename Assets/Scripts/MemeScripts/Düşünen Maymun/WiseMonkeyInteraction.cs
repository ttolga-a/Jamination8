using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WiseMonkeyInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider2D triggerZone;
    [SerializeField] private Player player;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip monkeySound;
    [SerializeField] private GameObject pressFText;

    [Header("Remaining Time Details")]
    [SerializeField] private RemainingTimeManager remainingTimeManager;
    [SerializeField] private float aValue;
    [SerializeField] private float bValue;


    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [TextArea]
    [SerializeField] private string firstDialogue =
        "Bilgelik zordur dostum... Ama denemek ister misin?";
    [TextArea]
    [SerializeField] private string secondDialogue =
        "Sen zaten bilgelik sınavını geçtin dostum! Artık yoluna devam et...";

    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float typeDuration = 3f;
    [SerializeField] private float dialogueHoldTime = 2f;
    [SerializeField] private string sceneToLoad = "MiniGameScene";

    private bool playerInRange = false;
    private bool isInteracting = false;
    private bool isTransitioning = false;

    void Awake()
    {
        remainingTimeManager = FindAnyObjectByType<RemainingTimeManager>();
    }
    private void Start()
    {
        if (pressFText) pressFText.SetActive(false);
        if (dialoguePanel) dialoguePanel.SetActive(false);

        if (fadeImage)
        {
            Color c = fadeImage.color;
            c.a = 0;
            fadeImage.color = c;
        }
    }

    private void Update()
    {
        if (isTransitioning || isInteracting) return;

        bool inside = triggerZone && triggerZone.bounds.Contains(player.transform.position);

        if (inside && !playerInRange)
        {
            playerInRange = true;
            if (pressFText) pressFText.SetActive(true);
        }
        else if (!inside && playerInRange)
        {
            playerInRange = false;
            if (pressFText) pressFText.SetActive(false);
            if (player != null && player.isLocked)
                player.UnlockPlayer();
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(MonkeyDialogueRoutine());
        }
    }

    private IEnumerator MonkeyDialogueRoutine()
    {
        isInteracting = true;

        if (player != null)
            player.LockPlayer();

        if (pressFText)
            pressFText.SetActive(false);

        if (audioSource && monkeySound)
        {
            audioSource.clip = monkeySound;
            audioSource.Play();
        }

        dialoguePanel.SetActive(true);
        dialogueText.text = "";

        bool alreadyDone = SceneDataManager.Instance != null && SceneDataManager.Instance.wiseMonkeyDone;

        string activeDialogue = alreadyDone ? secondDialogue : firstDialogue;
        float delay = typeDuration / activeDialogue.Length;

        foreach (char c in activeDialogue)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(dialogueHoldTime);
        dialoguePanel.SetActive(false);

        // Eğer zaten yapılmışsa sahne geçme
        if (alreadyDone)
        {
            if (player != null)
                player.UnlockPlayer();

            isInteracting = false;
            yield break;
        }

        // 🧠 İlk defa yapılıyorsa kaydet
        if (SceneDataManager.Instance != null)
        {
            SceneDataManager.Instance.wiseMonkeyDone = true;
            SceneDataManager.Instance.SavePlayerData(player.transform.position, SceneManager.GetActiveScene().name);
            SceneDataManager.Instance.SaveProgress(); // kalıcı hale getir
        }

        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));
        SceneManager.LoadScene(sceneToLoad);
    }

    private IEnumerator Fade(float start, float end, float duration)
    {
        isTransitioning = true;
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            Color c = fadeImage.color;
            c.a = Mathf.Lerp(start, end, time / duration);
            fadeImage.color = c;
            yield return null;
        }

        Color final = fadeImage.color;
        final.a = end;
        fadeImage.color = final;
    }
}
