using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlanktonInteraction : MonoBehaviour
{
    [Header("Interaction Elements")]
    public GameObject pressFText;       // “F tuşuna basın” yazısı (sahne objesi)
    public GameObject dialoguePanel;    // Konuşma paneli (Canvas içinde)
    public TMP_Text dialogueText;       // TMP Text (harf harf yazılacak)
    public string fullDialogue = "Bombaclat";  // Yazılacak metin

    [Header("Audio")]
    public AudioSource audioSource;     // Ses kaynağı
    public AudioClip talkSound;         // Konuşma sesi (örnek: plankton_talk.wav)

    [Header("Timing Settings")]
    public float typeDuration = 7f;     // Metnin toplamda yazılma süresi (7 sn)

    private bool playerInRange = false;
    private bool isTalking = false;

    private void Start()
    {
        if (pressFText)
            pressFText.SetActive(false);

        if (dialoguePanel)
            dialoguePanel.SetActive(false);
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
        }
    }

    private IEnumerator StartDialogue()
    {
        isTalking = true;

        // UI aç
        pressFText.SetActive(false);
        dialoguePanel.SetActive(true);
        dialogueText.text = "";

        // Ses başlat
        if (audioSource && talkSound)
        {
            audioSource.clip = talkSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        // Harf harf yazma
        float delayPerChar = typeDuration / fullDialogue.Length;
        foreach (char c in fullDialogue)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(delayPerChar);
        }

        // Ses durdur
        if (audioSource)
            audioSource.Stop();

        // 2 sn bekle sonra paneli kapat
        yield return new WaitForSeconds(2f);
        dialoguePanel.SetActive(false);

        isTalking = false;
    }
}
