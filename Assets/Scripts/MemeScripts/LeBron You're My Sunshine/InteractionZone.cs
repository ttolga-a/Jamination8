using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Interaction_Zone : MonoBehaviour
{
    [Header("Scene Elements")]
    [SerializeField] private Player player;             // 🔹 Player referansı (Player.cs)
    [SerializeField] private GameObject pressFObject;   // “F tuşuna basın” objesi
    [SerializeField] private Image displayImage;        // Ekranı kaplayacak UI Image
    [SerializeField] private AudioSource audioSource;   // Ses kaynağı
    [SerializeField] private AudioClip interactionSound;// Çalınacak ses

    [Header("Timing Settings")]
    [SerializeField] private float displayDuration = 5f;   // Görselin tam görünür kalma süresi
    [SerializeField] private float fadeDuration = 2f;      // Görsel fade süresi
    [SerializeField] private float soundDuration = 10f;    // Sesin toplam çalma süresi (fade in/out dahil)
    [SerializeField] private float soundFadeDuration = 2f; // Ses fade süresi (yükselme/alçalma)

    private bool playerInZone = false;
    private bool isInteracting = false;

    private void Start()
    {
        // Başlangıçta “F’ye bas” yazısı ve görsel kapalı
        if (pressFObject)
            pressFObject.SetActive(false);

        if (displayImage)
        {
            displayImage.gameObject.SetActive(false);
            Color c = displayImage.color;
            c.a = 0;
            displayImage.color = c;
        }
    }

    private void Update()
    {
        // 🔘 F tuşuna basıldığında ve şu anda etkileşim yoksa başlat
        if (playerInZone && Input.GetKeyDown(KeyCode.F) && !isInteracting)
        {
            StartCoroutine(InteractionSequence());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = true;

            if (pressFObject)
                pressFObject.SetActive(true);

            // Eğer ses daha önceden çalıyorsa sıfırla
            if (audioSource && audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = false;

            if (pressFObject)
                pressFObject.SetActive(false);

            // 🟢 Eğer oyuncu alanı terk ederse kontrolü geri ver
            if (player != null && player.isLocked)
                player.UnlockPlayer();
        }
    }

    private IEnumerator InteractionSequence()
    {
        isInteracting = true;

        // 🔒 Oyuncu hareket etmesin
        if (player != null)
            player.LockPlayer();

        // “F’ye bas” yazısını gizle
        if (pressFObject)
            pressFObject.SetActive(false);

        // 🎧 Ses çalmaya başla (paralel fade)
        if (audioSource && interactionSound)
            StartCoroutine(PlaySoundWithFade());

        // 🎬 Görseli aktif hale getir
        displayImage.gameObject.SetActive(true);

        // Fade-in (2 saniyede görünsün)
        yield return StartCoroutine(FadeImage(0f, 1f, fadeDuration));

        // Görsel 5 saniye boyunca tam görünür halde kalsın
        yield return new WaitForSeconds(displayDuration);

        // Fade-out (2 saniyede kaybolsun)
        yield return StartCoroutine(FadeImage(1f, 0f, fadeDuration));

        // Görseli tamamen kapat
        displayImage.gameObject.SetActive(false);

        // 🔓 Etkileşim bitti, oyuncuya kontrolü geri ver
        if (player != null && player.isLocked)
            player.UnlockPlayer();

        isInteracting = false;
    }

    // 🎨 Görsel fade in/out fonksiyonu
    private IEnumerator FadeImage(float startAlpha, float endAlpha, float duration)
    {
        float time = 0;
        Color imgColor = displayImage.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            imgColor.a = Mathf.Lerp(startAlpha, endAlpha, t);
            displayImage.color = imgColor;
            yield return null;
        }

        imgColor.a = endAlpha;
        displayImage.color = imgColor;
    }

    // 🎧 Ses fade in/out fonksiyonu (10 saniyede tam döngü)
    private IEnumerator PlaySoundWithFade()
    {
        audioSource.clip = interactionSound;
        audioSource.volume = 0f;
        audioSource.Play();

        // Fade-in (2 saniye)
        float fadeInTime = 0f;
        while (fadeInTime < soundFadeDuration)
        {
            fadeInTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, 1f, fadeInTime / soundFadeDuration);
            yield return null;
        }

        // Ses sabit çalsın (ortadaki süre)
        yield return new WaitForSeconds(soundDuration - (soundFadeDuration * 2f));

        // Fade-out (2 saniye)
        float fadeOutTime = 0f;
        while (fadeOutTime < soundFadeDuration)
        {
            fadeOutTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(1f, 0f, fadeOutTime / soundFadeDuration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = 1f; // Ses seviyesi reset
    }
}
