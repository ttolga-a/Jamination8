using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoomCharacterInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private GameObject pressFText;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip boomSound;

    [Header("Dialogue Settings")]
    [TextArea] [SerializeField] private string firstDialogue = "Heey! Dur bakalım, hemen patlatma!";
    [TextArea] [SerializeField] private string secondDialogue = "Boom! Benimle işin bitti dostum!";

    [Header("Timing Settings")]
    [SerializeField] private float typeDuration = 7f;
    [SerializeField] private float fadeDuration = 2f;


    [Header("Extra Collider (non-trigger)")]
    [SerializeField] private Collider2D physicalCollider; // isTrigger = false collider

    private bool playerInRange = false;
    private bool isInteracting = false;
    private bool hasFadedOut = false;
    private bool inPhysicalZone = false;
    private int interactionCount = 0;

    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;

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
        if ((playerInRange || inPhysicalZone) && Input.GetKeyDown(KeyCode.F) && !isInteracting)
        {
            interactionCount++;

            // ✅ İlk basış: her durumda ilk diyalog göster
            if (interactionCount == 1)
            {
                StartCoroutine(ShowFirstDialogueOnly());
            }
            // ✅ İkinci basış: tam etkileşim (ses + fade + collider silme)
            else if (interactionCount >= 2)
            {
                StartCoroutine(StartInteraction());
            }
        }
    }

    // 🧱 Fiziksel collider (isTrigger=false)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            inPhysicalZone = true;
            if (pressFText) pressFText.SetActive(true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            inPhysicalZone = false;
            if (pressFText) pressFText.SetActive(false);

            if (player != null && player.isLocked)
                player.UnlockPlayer();
        }
    }

    // 🟦 Trigger collider (ikinci etkileşim)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            if (pressFText) pressFText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            if (pressFText) pressFText.SetActive(false);

            if (hasFadedOut)
                StartCoroutine(FadeInCharacter());

            if (player != null && player.isLocked)
                player.UnlockPlayer();
        }
    }

    // 🟢 İlk diyalog — sadece yazı
    private IEnumerator ShowFirstDialogueOnly()
    {
        isInteracting = true;

        if (pressFText) pressFText.SetActive(false);
        if (player != null) player.LockPlayer();

        dialoguePanel.SetActive(true);
        dialogueText.text = "";

        float delay = typeDuration / firstDialogue.Length;
        foreach (char c in firstDialogue)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(2f);

        dialoguePanel.SetActive(false);

        if (player != null && player.isLocked)
            player.UnlockPlayer();

        isInteracting = false;
    }

    // 🔵 İkinci diyalog — ses + fade + collider silme
    private IEnumerator StartInteraction()
    {
        isInteracting = true;

        if (player != null) player.LockPlayer();
        if (pressFText) pressFText.SetActive(false);

        dialoguePanel.SetActive(true);
        dialogueText.text = "";

        float delayPerChar = typeDuration / secondDialogue.Length;
        foreach (char c in secondDialogue)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(delayPerChar);
        }

        yield return new WaitForSeconds(0.5f);

        if (audioSource && boomSound)
        {
            audioSource.clip = boomSound;
            audioSource.Play();
        }

        yield return new WaitForSeconds(boomSound.length);

        yield return StartCoroutine(FadeOutCharacter());

        dialoguePanel.SetActive(false);

        // 🧨 Collider'ı sadece component olarak sil (GameObject değil!)
        if (physicalCollider != null)
            Destroy(physicalCollider);

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

            foreach (var sr in spriteRenderers)
            {
                if (sr == null) continue; // 💡 null kontrolü
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
            }

            yield return null;
        }

        foreach (var sr in spriteRenderers)
        {
            if (sr == null) continue;
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

            foreach (var sr in spriteRenderers)
            {
                if (sr == null) continue;
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
            }

            yield return null;
        }

        foreach (var sr in spriteRenderers)
        {
            if (sr == null) continue;
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
        }

        hasFadedOut = false;
    }
}
