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
    [SerializeField] private Transform player;             // Player Transform
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

        audioSource.playOnAwake = false;
        audioSource.loop = true;
    }

    private void Update()
    {
        if (player == null) return;

        // 🎧 Mesafe kontrolü (şarkı ses seviyesi)
        float distance = Vector2.Distance(transform.position, player.position);
        float volume = Mathf.Clamp01(1 - (distance / maxHearingDistance));
        audioSource.volume = volume;

        // Şarkı çalması kontrolü
        if (!isSinging && volume > 0.01f && !isTalking)
        {
            StartSinging();
        }
        else if (isSinging && volume <= 0.01f)
        {
            StopSinging();
        }

        // 🔹 Player collider’a giriyor mu?
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
            }
        }

        // 🔘 F tuşu konuşmayı başlatır
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
        audioSource.Stop();
    }

    private IEnumerator StartDialogue()
    {
        isTalking = true;
        StopSinging();
        pressFText.SetActive(false);
        dialoguePanel.SetActive(true);
        dialogueText.text = "";
        animator.Play("pee");

        // Harf harf yaz
        float delay = typeDuration / fullDialogue.Length;
        foreach (char c in fullDialogue)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(delay);
        }

        // Konuşma sesi çal
        if (talkingClip)
        {
            audioSource.clip = talkingClip;
            audioSource.loop = false;
            audioSource.volume = 1f;
            audioSource.Play();
        }

        // Konuşma süresi kadar bekle
        yield return new WaitForSeconds(talkingClip.length);

        // Konuşma bitince
        dialoguePanel.SetActive(false);
        animator.Play("idle");
        isTalking = false;
        StartSinging();
    }

    private bool IsPlayerInsideTrigger()
{
    // Player gerçekten triggerCollider içinde mi?
    if (triggerZone == null || player == null)
        return false;

    // OverlapBox ile alan içinde mi kontrol et
    Collider2D hit = Physics2D.OverlapBox(
        triggerZone.bounds.center,
        triggerZone.bounds.size,
        0,
        LayerMask.GetMask("Player") // sadece Player layer’ına bak
    );

    return hit != null;
}

}
