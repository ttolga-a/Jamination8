using System.Collections;
using UnityEngine;
using TMPro;

public class SingingCharacterSerialized : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource audioSource;      // Tek AudioSource
    [SerializeField] private AudioClip singingClip;        // Şarkı sesi
    [SerializeField] private AudioClip talkingClip;        // Konuşma sesi
    [SerializeField] private Animator animator;            // Idle - Pee animasyonları
    [SerializeField] private Collider2D triggerZone;       // Etkileşim alanı
    [SerializeField] private Player player;                // 🔒 Player referansı
    [SerializeField] private GameObject pressFText;        // “F’ye bas” UI objesi
    [SerializeField] private GameObject dialoguePanel;     // Konuşma paneli
    [SerializeField] private TMP_Text dialogueText;        // TMP Text alanı
    [TextArea] [SerializeField] private string fullDialogue = "Bombaclat! Mikrofon bende!";

    [Header("Settings")]
    [SerializeField] private float maxHearingDistance = 10f;   // 10 birimden duyulacak
    [SerializeField] private float typeDuration = 7f;          // Harf harf yazma süresi

    private bool playerInRange = false;
    private bool isTalking = false;
    private bool isSinging = false;

    private void Start()
    {
        if (pressFText) pressFText.SetActive(false);
        if (dialoguePanel) dialoguePanel.SetActive(false);

        if (audioSource)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = true;
        }
    }

    private void Update()
    {
        if (player == null) return;

        // 🎧 Ses mesafesine göre volume ayarla
        float distance = Vector2.Distance(transform.position, player.transform.position);
        float volume = Mathf.Clamp01(1 - (distance / maxHearingDistance));
        audioSource.volume = volume;

        // Şarkı kontrolü
        if (!isSinging && volume > 0.01f && !isTalking)
        {
            StartSinging();
        }
        else if (isSinging && volume <= 0.01f)
        {
            StopSinging();
        }

        // 🔹 Player trigger alanında mı?
        if (triggerZone != null && IsPlayerInsideTrigger())
        {
            if (!playerInRange)
            {
                playerInRange = true;
                if (pressFText) pressFText.SetActive(true);
            }
        }
        else
        {
            if (playerInRange)
            {
                playerInRange = false;
                if (pressFText) pressFText.SetActive(false);

                // 🔓 Eğer alan dışına çıktıysa kontrolü geri ver
                if (player != null && player.isLocked)
                    player.UnlockPlayer();
            }
        }

        // 🔘 F tuşuna basınca konuşma başlasın
        if (playerInRange && Input.GetKeyDown(KeyCode.F) && !isTalking)
        {
            StartCoroutine(StartDialogue());
        }
    }

    private void StartSinging()
    {
        isSinging = true;
        audioSource.clip = singingClip;
        audioSource.loop = true;
        audioSource.Play();
        animator.Play("idle");
    }

    private void StopSinging()
    {
        isSinging = false;
        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    private IEnumerator StartDialogue()
    {
        isTalking = true;
        StopSinging();
        if (pressFText) pressFText.SetActive(false);

        // 🔒 Oyuncunun kontrolünü kilitle
        if (player != null)
            player.LockPlayer();

        // UI aç
        dialoguePanel.SetActive(true);
        dialogueText.text = "";
        animator.Play("pee");

        // ✍️ Harf harf yazdır
        float delay = typeDuration / fullDialogue.Length;
        foreach (char c in fullDialogue)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(delay);
        }

        // 🎧 Konuşma sesi çal
        if (talkingClip)
        {
            audioSource.clip = talkingClip;
            audioSource.loop = false;
            audioSource.volume = 1f;
            audioSource.Play();
        }

        // Konuşma sesi bitene kadar bekle
        yield return new WaitForSeconds(talkingClip.length);

        // UI kapat, idle'a dön
        dialoguePanel.SetActive(false);
        animator.Play("idle");

        // 🔓 Player kontrolünü geri ver
        if (player != null && player.isLocked)
            player.UnlockPlayer();

        isTalking = false;

        // Konuşma bittikten sonra şarkı tekrar başlasın
        StartSinging();
    }

    private bool IsPlayerInsideTrigger()
    {
        if (triggerZone == null || player == null)
            return false;

        Collider2D hit = Physics2D.OverlapBox(
            triggerZone.bounds.center,
            triggerZone.bounds.size,
            0,
            LayerMask.GetMask("Player")
        );

        return hit != null;
    }
}
