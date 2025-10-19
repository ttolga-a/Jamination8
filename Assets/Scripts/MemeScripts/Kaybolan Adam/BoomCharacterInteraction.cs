using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoomCharacterInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;             // 🔹 Player referansı
    [SerializeField] private GameObject pressFText;     // “F tuşuna basın” yazısı
    [SerializeField] private GameObject dialoguePanel;  // Canvas'taki konuşma paneli
    [SerializeField] private TMP_Text dialogueText;     // Konuşma yazısı

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;   // Ses kaynağı
    [SerializeField] private AudioClip boomSound;       // 1 sn’lik boom sesi

    [Header("Dialogue Settings")]
    [TextArea]
    [SerializeField] private string fullDialogue = "Boom! Benimle işin bitti dostum!";

    [Header("Timing Settings")]
    [SerializeField] private float typeDuration = 7f;   // Harf harf yazılma süresi
    [SerializeField] private float fadeDuration = 2f;   // Karakterin solma süresi

    [Header("Remaining Time Details")]
    [SerializeField] private RemainingTimeManager remainingTimeManager;
    [SerializeField] private float aValue;
    [SerializeField] private float bValue;

    private bool playerInRange = false;
    private bool isInteracting = false;
    private bool hasFadedOut = false;

    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;

    void Awake()
    {
        remainingTimeManager = FindAnyObjectByType<RemainingTimeManager>();
    }
    private void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        originalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
            originalColors[i] = spriteRenderers[i].color;

        if (pressFText) pressFText.SetActive(false);
        if (dialoguePanel) dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F) && !isInteracting)
        {
            StartCoroutine(StartInteraction());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            if (pressFText)
                pressFText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            if (pressFText)
                pressFText.SetActive(false);

            if (hasFadedOut)
                StartCoroutine(FadeInCharacter());

            // 🔓 Güvenlik: oyuncu alandan çıkarsa kontrolü geri ver
            if (player != null && player.isLocked)
                player.UnlockPlayer();
        }
    }

    private IEnumerator StartInteraction()
    {
        isInteracting = true;

        // 🔒 Oyuncu hareket etmesin
        if (player != null)
            player.LockPlayer();

        if (pressFText)
            pressFText.SetActive(false);

        dialoguePanel.SetActive(true);
        dialogueText.text = "";

        // ✍️ Harf harf yazdır
        float delayPerChar = typeDuration / fullDialogue.Length;
        foreach (char c in fullDialogue)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(delayPerChar);
        }

        yield return new WaitForSeconds(0.5f);

        // 🎧 Ses çal
        if (audioSource && boomSound)
        {
            audioSource.clip = boomSound;
            audioSource.Play();
        }

        yield return new WaitForSeconds(boomSound.length);

        // 🌫️ Karakter fade-out (kaybolma)
        yield return StartCoroutine(FadeOutCharacter());

        dialoguePanel.SetActive(false);

        // 🔓 Kontrolü geri ver
        if (player != null && player.isLocked)
            player.UnlockPlayer();

        isInteracting = false;
    }

    private IEnumerator FadeOutCharacter()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0)
            yield break;

        float time = 0;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                Color c = originalColors[i];
                c.a = alpha;
                spriteRenderers[i].color = c;
            }

            yield return null;
        }

        foreach (var sr in spriteRenderers)
        {
            Color c = sr.color;
            c.a = 0f;
            sr.color = c;
        }

        hasFadedOut = true;
    }

    private IEnumerator FadeInCharacter()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0)
            yield break;

        float time = 0;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                Color c = originalColors[i];
                c.a = alpha;
                spriteRenderers[i].color = c;
            }

            yield return null;
        }

        foreach (var sr in spriteRenderers)
        {
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
        }

        hasFadedOut = false;
    }
}
