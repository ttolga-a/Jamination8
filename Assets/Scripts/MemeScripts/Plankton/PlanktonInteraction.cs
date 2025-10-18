using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlanktonInteraction : MonoBehaviour
{
    [Header("Interaction Elements")]
    [SerializeField] private GameObject pressFText;     // “F tuşuna basın” yazısı
    [SerializeField] private GameObject dialoguePanel;  // Konuşma paneli
    [SerializeField] private TMP_Text dialogueText;     // TMP Text (harf harf yazılacak)
    [TextArea] [SerializeField] private string fullDialogue = "Bombaclat!"; // Diyalog metni

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;   // Ses kaynağı
    [SerializeField] private AudioClip talkSound;       // Konuşma sesi

    [Header("Timing Settings")]
    [SerializeField] private float typeDuration = 7f;   // Yazı süresi (7 sn)
    [SerializeField] private float endDelay = 2f;       // Bitince bekleme süresi

    [Header("References")]
    [SerializeField] private Player player;             // 🔹 Player referansı (Player.cs)

    private bool playerInRange = false;
    private bool isTalking = false;

    private void Start()
    {
        if (pressFText) pressFText.SetActive(false);
        if (dialoguePanel) dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F) && !isTalking)
        {
            StartCoroutine(StartDialogue());
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

            // Eğer konuşma sırasında çıkarsa player kontrolü geri al
            if (player && player.isLocked)
                player.UnlockPlayer();
        }
    }

    private IEnumerator StartDialogue()
    {
        isTalking = true;

        // 🔒 Player hareket etmesin
        if (player)
            player.LockPlayer();

        // UI aktif
        if (pressFText) pressFText.SetActive(false);
        if (dialoguePanel) dialoguePanel.SetActive(true);
        dialogueText.text = "";

        // 🎵 Ses başlat
        if (audioSource && talkSound)
        {
            audioSource.clip = talkSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        // ✍️ Harf harf yazdırma
        float delayPerChar = typeDuration / fullDialogue.Length;
        foreach (char c in fullDialogue)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(delayPerChar);
        }

        // 🔇 Ses durdur
        if (audioSource)
            audioSource.Stop();

        // 🕒 2 sn bekle
        yield return new WaitForSeconds(endDelay);

        // UI kapat
        if (dialoguePanel)
            dialoguePanel.SetActive(false);

        // 🔓 Player’ı tekrar aktif et
        if (player && player.isLocked)
            player.UnlockPlayer();

        isTalking = false;
    }
}
