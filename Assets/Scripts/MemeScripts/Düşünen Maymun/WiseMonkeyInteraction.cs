using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WiseMonkeyInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider2D triggerZone;    // Etkileşim alanı
    [SerializeField] private Player player;             // 🔹 Player referansı (Player.cs)
    [SerializeField] private AudioSource audioSource;   // Ses kaynağı
    [SerializeField] private AudioClip monkeySound;     // Maymun sesi
    [SerializeField] private GameObject pressFText;     // “F’ye bas” yazısı

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;  // Konuşma paneli
    [SerializeField] private TMP_Text dialogueText;     // TMP Text objesi
    [TextArea]
    [SerializeField] private string monkeyDialogue = 
        "Bilgelik zordur dostum... Ama denemek ister misin?"; // Maymunun sözü

    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float typeDuration = 3f;        // Yazı harf harf süresi
    [SerializeField] private float dialogueHoldTime = 2f;    // Yazı ekranda kalma süresi
    [SerializeField] private string sceneToLoad = "MiniGameScene";

    private bool playerInRange = false;
    private bool isInteracting = false;
    private bool isTransitioning = false;

    private void Start()
    {
        if (pressFText) pressFText.SetActive(false);
        if (dialoguePanel) dialoguePanel.SetActive(false);

        // Fade image başlangıçta görünmez olsun
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

        // 🎯 Oyuncu alana girerse “F’ye bas” yazısı çıksın
        if (inside && !playerInRange)
        {
            playerInRange = true;
            if (pressFText) pressFText.SetActive(true);
        }
        else if (!inside && playerInRange)
        {
            playerInRange = false;
            if (pressFText) pressFText.SetActive(false);

            // 🔓 Oyuncu alanı terk ederse kontrol geri gelsin
            if (player != null && player.isLocked)
                player.UnlockPlayer();
        }

        // 🔘 F tuşu ile etkileşimi başlat
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(MonkeyDialogueRoutine());
        }
    }

    private IEnumerator MonkeyDialogueRoutine()
    {
        isInteracting = true;

        // 🔒 Oyuncuyu kilitle (hareket etmesin)
        if (player != null)
            player.LockPlayer();

        // “F’ye bas” yazısını gizle
        if (pressFText)
            pressFText.SetActive(false);

        // 🎵 Maymun sesi çal
        if (audioSource && monkeySound)
        {
            audioSource.clip = monkeySound;
            audioSource.Play();
        }

        // 🗨️ Diyalog panelini aç
        dialoguePanel.SetActive(true);
        dialogueText.text = "";

        // Harf harf yazı efekti
        float delay = typeDuration / monkeyDialogue.Length;
        foreach (char c in monkeyDialogue)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(delay);
        }

        // Yazı tamamlandıktan sonra biraz bekle
        yield return new WaitForSeconds(dialogueHoldTime);

        // Diyalog panelini kapat
        dialoguePanel.SetActive(false);

        // 🎬 Fade-out başlat
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        // 🔓 Yeni sahneye geçmeden önce oyuncuyu serbest bırak (opsiyonel güvenlik)
        if (player != null && player.isLocked)
            player.UnlockPlayer();

        // 🎮 Yeni sahneye geç
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

        // Son değeri garantiye al
        Color final = fadeImage.color;
        final.a = end;
        fadeImage.color = final;
    }
}
