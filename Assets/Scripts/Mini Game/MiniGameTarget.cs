using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MiniGameTarget : MonoBehaviour
{
    private Button button;
    private float spawnTime;
    public float lifetime = 2f;

    [Header("Ses ve Animasyon")]
    public AudioClip clickSound;
    private AudioSource audioSource;
    public float popScaleMultiplier = 1.2f;
    public float popDuration = 0.1f;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClicked);

        audioSource = gameObject.AddComponent<AudioSource>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        spawnTime = Time.time;
        StartCoroutine(AutoDestroy());
    }

    private void OnClicked()
    {
        if (!MiniGameManager.Instance || !MiniGameManager.Instance.gameActive) return;
        // Ses efekti
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound);

        MiniGameManager.Instance.AddScore();


        // Pop animasyon
        StartCoroutine(PopAnimation());

        // Fade out ve destroy
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(lifetime);
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator PopAnimation()
    {
        Vector3 originalScale = rectTransform.localScale;
        Vector3 targetScale = originalScale * popScaleMultiplier;
        float t = 0f;

        // Scale up
        while (t < popDuration)
        {
            t += Time.deltaTime;
            rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, t / popDuration);
            yield return null;
        }

        // Scale down
        t = 0f;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, t / popDuration);
            yield return null;
        }

        rectTransform.localScale = originalScale;
    }

    private IEnumerator FadeOutAndDestroy()
    {
        float t = 0f;
        float duration = 0.25f;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / duration);
            yield return null;
        }

        Destroy(gameObject);
    }
}
