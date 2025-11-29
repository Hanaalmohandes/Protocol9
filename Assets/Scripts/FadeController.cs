using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public Image fadeImage;

    void Awake()
    {
        if (fadeImage == null)
            fadeImage = GetComponent<Image>();
    }

    public IEnumerator FadeOut(float duration = 1f)
    {
        float t = 0;
        Color c = fadeImage.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / duration);
            fadeImage.color = c;
            yield return null;
        }
    }
}