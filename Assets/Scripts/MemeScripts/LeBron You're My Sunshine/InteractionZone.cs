using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Interaction_Zone : MonoBehaviour
{
    [Header("Scene Elements")]
    public GameObject pressFObject;     // Sahnedeki “F tuşuna basın” objesi
    public Image displayImage;          // Ekranı kaplayacak UI Image (Canvas içinde)
    public AudioSource audioSource;     // Ses kaynağı
    public AudioClip interactionSound;  // Çalınacak ses

    [Header("Timing Settings")]
    public float displayDuration = 5f;  // Görselin tam görünür kalma süresi
    public float fadeDuration = 2f;     // Görsel fade süresi
    public float soundDuration = 10f;   // Sesin toplam çalma süresi (fade in/out dahil)
    public float soundFadeDuration = 2f; // Ses fade süresi (yükselme/alçalma)

    private bool playerInZone = false;
    private bool isInteracting = false;

    private void Start()
    {
        if (pressFObject)
            pressFObject.SetActive(false);

        if (displayImage)
        {
            displayImage.gameObject.SetActive(false);
            Color c = displayImage.color;
            c.a = 0;
            displayImage.color = c; // Başta tamamen şeffaf
        }
    }

    private void Update()
    {
        if (playerInZone && Input.GetKeyDown(KeyCode.F) && !isInteracting)
        {
            StartCoroutine(ShowImageRoutine());
            if (audioSource && interactionSound)
            {
                StartCoroutine(PlaySoundWithFade());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = true;
            if (pressFObject)
                pressFObject.SetActive(true);

            // Ses resetlensin (bölünmesin)
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
        }
    }

    private IEnumerator ShowImageRoutine()
    {
        isInteracting = true;

        if (pressFObject)
            pressFObject.SetActive(false);

        displayImage.gameObject.SetActive(true);

        // Fade-in (2 saniyede görünür hale gelsin)
        yield return StartCoroutine(FadeImage(0f, 1f, fadeDuration));

        // Görsel tam görünür halde 5 saniye bekle
        yield return new WaitForSeconds(displayDuration);

        // Fade-out (2 saniyede kaybolsun)
        yield return StartCoroutine(FadeImage(1f, 0f, fadeDuration));

        displayImage.gameObject.SetActive(false);

        isInteracting = false;
    }

    private IEnumerator FadeImage(float startAlpha, float endAlpha, float duration)
    {
        float time = 0;
        Color imgColor = displayImage.color;

        while (time < duration)
        {
            float t = time / duration;
            imgColor.a = Mathf.Lerp(startAlpha, endAlpha, t);
            displayImage.color = imgColor;
            time += Time.deltaTime;
            yield return null;
        }

        imgColor.a = endAlpha;
        displayImage.color = imgColor;
    }

    // 🎧 SES FONKSİYONU (10 saniye, fade-in/out dâhil)
    private IEnumerator PlaySoundWithFade()
    {
        audioSource.clip = interactionSound;
        audioSource.volume = 0f;
        audioSource.Play();

        // FADE IN (ilk 2 saniye)
        float fadeInTime = 0f;
        while (fadeInTime < soundFadeDuration)
        {
            fadeInTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, 1f, fadeInTime / soundFadeDuration);
            yield return null;
        }

        // ORTA KISIM (ses sabit çalsın)
        yield return new WaitForSeconds(soundDuration - (soundFadeDuration * 2f));

        // FADE OUT (son 2 saniye)
        float fadeOutTime = 0f;
        while (fadeOutTime < soundFadeDuration)
        {
            fadeOutTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(1f, 0f, fadeOutTime / soundFadeDuration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = 1f; // Sonraki oynatmalar için sıfırlansın
    }
}
