using System.Collections;
using UnityEngine;
using TMPro;

public class SingingCharacterSerialized : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource audioSource;      
    [SerializeField] private AudioClip singingClip;        
    [SerializeField] private AudioClip talkingClip;        
    [SerializeField] private Animator animator;            
    [SerializeField] private Collider2D triggerZone;       
    [SerializeField] private Player player;                
    [SerializeField] private GameObject pressFText;        
    [SerializeField] private GameObject dialoguePanel;     
    [SerializeField] private TMP_Text dialogueText;        

    [Header("Dialogue Texts")]
    [TextArea][SerializeField] private string firstDialogue = "Yo dostum! Müzik kulağını aç!";
    [TextArea][SerializeField] private string secondDialogue = "Bombaclat! Mikrofon bende!";

    [Header("Settings")]
    [SerializeField] private float maxHearingDistance = 10f;   
    [SerializeField] private float typeDuration = 7f;          

    private bool playerInRange = false;
    private bool isTalking = false;
    private bool isSinging = false;

    private int interactionCount = 0; // 🧠 kaç kez F basıldı
    
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

        float distance = Vector2.Distance(transform.position, player.transform.position);
        float volume = Mathf.Clamp01(1 - (distance / maxHearingDistance));
        audioSource.volume = volume;

        if (!isSinging && volume > 0.01f && !isTalking)
        {
            StartSinging();
        }
        else if (isSinging && volume <= 0.01f)
        {
            StopSinging();
        }

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
                if (player != null && player.isLocked)
                    player.UnlockPlayer();
            }
        }

        // 🔘 F tuşuna basınca
        if (playerInRange && Input.GetKeyDown(KeyCode.F) && !isTalking)
        {
            interactionCount++;

            if (interactionCount == 1)
            {
                // 🔹 İlk kez: sadece kısa diyalog (şarkı ve animasyon bozulmadan)
                StartCoroutine(ShowFirstDialogueOnly());
            }
            else
            {
                // 🔹 İkinci ve sonrası: normal diyalog süreci
                StartCoroutine(StartDialogue());
            }
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

    // 🔹 İlk F’ye basıldığında sadece yazı çıkacak, müzik kesilmeyecek
    private IEnumerator ShowFirstDialogueOnly()
    {
        isTalking = true;
        if (pressFText) pressFText.SetActive(false);

        if (player != null)
            player.LockPlayer();

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

        isTalking = false;
    }

    // 🔹 İkinci ve sonraki F basışlarında tam süreç (animasyon + konuşma sesi)
    private IEnumerator StartDialogue()
    {
        isTalking = true;
        StopSinging();
        if (pressFText) pressFText.SetActive(false);

        if (player != null)
            player.LockPlayer();

        dialoguePanel.SetActive(true);
        dialogueText.text = "";
        animator.Play("pee");

        float delay = typeDuration / secondDialogue.Length;
        foreach (char c in secondDialogue)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(delay);
        }

        if (talkingClip)
        {
            audioSource.clip = talkingClip;
            audioSource.loop = false;
            audioSource.volume = 1f;
            audioSource.Play();
        }

        yield return new WaitForSeconds(talkingClip.length);

        dialoguePanel.SetActive(false);
        animator.Play("idle");

        if (player != null && player.isLocked)
            player.UnlockPlayer();

        isTalking = false;

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
