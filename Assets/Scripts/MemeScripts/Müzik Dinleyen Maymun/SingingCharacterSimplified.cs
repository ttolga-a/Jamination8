using UnityEngine;

public class SingingCharacterSimplified : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource audioSource;   // Ses kaynağı
    [SerializeField] private AudioClip singingClip;     // Sürekli çalacak ses
    [SerializeField] private Player player;             // Oyuncu referansı

    [Header("Settings")]
    [SerializeField] private float maxHearingDistance = 10f;  // Ses duyulma mesafesi

    private void Start()
    {
        if (audioSource)
        {
            audioSource.clip = singingClip;
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            audioSource.volume = 0f;
            audioSource.Play(); // Başta başlasın
        }
    }

    private void Update()
    {
        if (player == null || audioSource == null)
            return;

        // 🎧 Mesafeye göre ses şiddetini ayarla
        float distance = Vector2.Distance(transform.position, player.transform.position);
        float volume = Mathf.Clamp01(1 - (distance / maxHearingDistance));
        audioSource.volume = volume;
    }
}
