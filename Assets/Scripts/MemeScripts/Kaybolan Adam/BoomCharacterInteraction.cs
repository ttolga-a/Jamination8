using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoomCharacterInteraction : MonoBehaviour
{
    [Header("Interaction Elements")]
    public GameObject pressFText;       // “F tuşuna basın” yazısı (sahnede)
    public GameObject dialoguePanel;    // Canvas'taki konuşma paneli
    public TMP_Text dialogueText;       // Konuşma yazısı
    [TextArea]
    public string fullDialogue = "Boom! Benimle işin bitti dostum!"; // Yazılacak metin

    [Header("Audio")]
    public AudioSource audioSource;     // Ses kaynağı
    public AudioClip boomSound;         // 1 sn’lik boom sesi

    [Header("Timing Settings")]
    public float typeDuration = 7f;     // Metnin harf harf yazılma süresi
    public float fadeDuration = 2f;     // Karakterin solma ve geri gelme süresi

    private bool playerInRange = false;
    private bool isInteracting = false;
    private bool hasFadedOut = false;   // Karakter gerçekten yok oldu mu?

    private SpriteRenderer[] spriteRenderers; // Child dahil tüm SpriteRenderer'lar
    private Color[] originalColors;           // Orijinal renkleri saklamak için

    private void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
            originalColors[i] = spriteRenderers[i].color;

        if (pressFText)
            pressFText.SetActive(false);

        if (dialoguePanel)
            dialoguePanel.SetActive(false);
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

            // Sadece karakter gerçekten yok olmuşsa geri gelsin
            if (hasFadedOut)
                StartCoroutine(FadeInCharacter());
        }
    }

    private IEnumerator StartInteraction()
    {
        isInteracting = true;

        // “F tuşuna basın” yazısını gizle
        if (pressFText)
            pressFText.SetActive(false);

        // Paneli göster
        dialoguePanel.SetActive(true);
        dialogueText.text = "";

        // 1️⃣ Harf harf yazdırma
        float delayPerChar = typeDuration / fullDialogue.Length;
        foreach (char c in fullDialogue)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(delayPerChar);
        }

        // 2️⃣ Yazı bittikten sonra 1 saniyelik ses çal
        yield return new WaitForSeconds(0.5f);
        if (audioSource && boomSound)
        {
            audioSource.clip = boomSound;
            audioSource.Play();
        }

        yield return new WaitForSeconds(boomSound.length);

        // 3️⃣ Karakter fade-out (kaybolma)
        yield return StartCoroutine(FadeOutCharacter());

        // Paneli kapat
        dialoguePanel.SetActive(false);

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

        // Tamamen görünmez
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            Color c = originalColors[i];
            c.a = 0f;
            spriteRenderers[i].color = c;
        }

        hasFadedOut = true; // ✅ Artık karakter kayboldu
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

        // Tamamen görünür hale gelsin
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            Color c = originalColors[i];
            c.a = 1f;
            spriteRenderers[i].color = c;
        }

        hasFadedOut = false; // ✅ Artık karakter tekrar görünür
    }
}
