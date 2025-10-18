using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SingingCharacterController : MonoBehaviour
{
    [Header("References")]
    public Transform player;                 // Player referansı
    public Animator animator;                // Animator (idle ve pee)
    public AudioSource singingAudio;         // Şarkı sesi (loop)
    public AudioSource talkingAudio;         // Konuşma sesi (tek sefer)
    public AudioClip talkingClip;            // Konuşma sesi klibi
    public GameObject pressFText;            // “F’ye bas” yazısı
    public GameObject dialoguePanel;         // Canvas içindeki panel
    public TMP_Text dialogueText;            // TMP Text (yazı alanı)

    [Header("Dialogue Settings")]
    [TextArea] public string dialogue = "Pee time! Hemen geliyorum!";
    public float typeDuration = 7f;          // Yazı süresi

    private bool playerInDialogueZone = false;
    private bool isTalking = false;
    private bool isSinging = false;

    private void Start()
    {
        if (pressFText)
            pressFText.SetActive(false);

        if (dialoguePanel)
            dialoguePanel.SetActive(false);

        // Şarkı başta otomatik başlamasın, sadece trigger'da
        singingAudio.loop = true;
        singingAudio.playOnAwake = false;

        animator.Play("idle");
    }

    private void Update()
    {
        // 💬 Küçük collider içindeyken F’ye basılırsa
        if (playerInDialogueZone && Input.GetKeyDown(KeyCode.F) && !isTalking)
        {
            StartCoroutine(StartDialogue());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // 🎵 Büyük collider (şarkı bölgesi)
        if (collision.CompareTag("SingingZone"))
        {
            if (!singingAudio.isPlaying)
            {
                singingAudio.Play();
                isSinging = true;
                animator.Play("idle");
            }
        }

        // 💬 Küçük collider (diyalog bölgesi)
        if (collision.CompareTag("DialogueZone"))
        {
            playerInDialogueZone = true;
            if (pressFText)
                pressFText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // 🚶 Büyük collider'dan çıkarsa şarkı durur
        if (collision.CompareTag("SingingZone"))
        {
            singingAudio.Stop();
            isSinging = false;
        }

        // 🚶 Küçük collider'dan çıkarsa
        if (collision.CompareTag("DialogueZone"))
        {
            playerInDialogueZone = false;
            if (pressFText)
                pressFText.SetActive(false);

            // Eğer konuşma devam ediyorsa durdur
            if (isTalking)
            {
                StopAllCoroutines();
                if (talkingAudio.isPlaying)
                    talkingAudio.Stop();

                dialoguePanel.SetActive(false);
                isTalking = false;
            }

            // Karakter idle animasyonuna dön
            animator.Play("idle");

            // Şarkı tekrar başlasın
            if (!singingAudio.isPlaying)
            {
                singingAudio.Play();
                isSinging = true;
            }
        }
    }

    // 💬 Diyalog sistemi
    private IEnumerator StartDialogue()
    {
        isTalking = true;

        if (pressFText)
            pressFText.SetActive(false);

        // Şarkıyı durdur
        if (singingAudio.isPlaying)
        {
            singingAudio.Stop();
            isSinging = false;
        }

        // Animasyonu pee'ye geç
        animator.Play("pee");

        // Paneli aç
        dialoguePanel.SetActive(true);
        dialogueText.text = "";

        // Yazıyı harf harf yaz
        float delay = typeDuration / dialogue.Length;
        foreach (char c in dialogue)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(delay);
        }

        // Konuşma sesini çal
        if (talkingAudio && talkingClip)
        {
            talkingAudio.clip = talkingClip;
            talkingAudio.Play();
        }

        // Ses bitene kadar bekle
        yield return new WaitForSeconds(talkingClip.length);

        // Paneli kapat
        dialoguePanel.SetActive(false);
        isTalking = false;
    }
}
