using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MiniGameSceneFade : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    private void Start()
    {
        if (fadeImage != null)
            StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float t = 0f;
        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 0f;
        fadeImage.color = c;

        // istenirse en sonunda Image'ı kapat
        fadeImage.gameObject.SetActive(false);
    }
}
