using System.Collections;
using UnityEngine;

public class CrateSearch : MonoBehaviour
{
    public bool containsAccessCard = false;
    public GameObject accessCardPrefab;
    public Transform spawnPoint;

    public KeyCode searchKey = KeyCode.E;
    // Optional world-space hint sprite (assign a child GameObject with a SpriteRenderer)
    public GameObject hintSprite;
    // Fade settings for the hint (seconds)
    public float hintFadeDuration = 0.2f;

    // Internal cached components for fading
    private Coroutine hintFadeCoroutine;
    private SpriteRenderer hintSpriteRenderer;
    private CanvasGroup hintCanvasGroup;

    void Start()
    {
        if (hintSprite != null)
        {
            hintSpriteRenderer = hintSprite.GetComponent<SpriteRenderer>();
            hintCanvasGroup = hintSprite.GetComponent<CanvasGroup>();

            // Initialize to invisible and disabled so fade-in works predictably
            if (hintCanvasGroup != null)
            {
                hintCanvasGroup.alpha = 0f;
                hintSprite.SetActive(false);
            }
            else if (hintSpriteRenderer != null)
            {
                var c = hintSpriteRenderer.color;
                c.a = 0f;
                hintSpriteRenderer.color = c;
                hintSprite.SetActive(false);
            }
            else
            {
                hintSprite.SetActive(false);
            }
        }
    }

    public void SearchCrate()
    {
        Debug.Log("Crate searched!");

        if (containsAccessCard)
        {
            Instantiate(accessCardPrefab, spawnPoint.position, Quaternion.identity);
            containsAccessCard = false;
            Debug.Log("Access card spawned!");
        }
    }

    // Crate no longer controls animations; PlayerController owns the search animation state.

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player entered crate zone");
            if (hintSprite != null)
                ShowHint(true);
            var pc = collision.GetComponentInParent<PlayerController>();
            if (pc != null)
                pc.SetCurrentInteractable(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player left crate zone");
            if (hintSprite != null)
                ShowHint(false);
            var pc = collision.GetComponentInParent<PlayerController>();
            if (pc != null)
                pc.ClearCurrentInteractable(this);
        }
    }

    private void ShowHint(bool show)
    {
        if (hintSprite == null) return;

        // Stop any running fade
        if (hintFadeCoroutine != null)
        {
            StopCoroutine(hintFadeCoroutine);
            hintFadeCoroutine = null;
        }

        if (show)
        {
            // Ensure active then fade in
            hintSprite.SetActive(true);
            if (hintCanvasGroup != null)
            {
                hintCanvasGroup.alpha = 0f;
                hintFadeCoroutine = StartCoroutine(FadeCanvasGroup(hintCanvasGroup, 1f));
            }
            else if (hintSpriteRenderer != null)
            {
                var c = hintSpriteRenderer.color;
                c.a = 0f;
                hintSpriteRenderer.color = c;
                hintFadeCoroutine = StartCoroutine(FadeSprite(hintSpriteRenderer, 1f));
            }
        }
        else
        {
            // Fade out then disable
            if (hintCanvasGroup != null)
                hintFadeCoroutine = StartCoroutine(FadeOutCanvasThenDisable(hintCanvasGroup));
            else if (hintSpriteRenderer != null)
                hintFadeCoroutine = StartCoroutine(FadeOutSpriteThenDisable(hintSpriteRenderer));
            else
                hintSprite.SetActive(false);
        }
    }

    private IEnumerator FadeSprite(SpriteRenderer sr, float targetAlpha)
    {
        float start = sr.color.a;
        float t = 0f;
        while (t < hintFadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(start, targetAlpha, Mathf.Clamp01(t / hintFadeDuration));
            var c = sr.color;
            c.a = a;
            sr.color = c;
            yield return null;
        }
        var final = sr.color;
        final.a = targetAlpha;
        sr.color = final;
        hintFadeCoroutine = null;
    }

    private IEnumerator FadeOutSpriteThenDisable(SpriteRenderer sr)
    {
        yield return FadeSprite(sr, 0f);
        if (hintSprite != null)
            hintSprite.SetActive(false);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float targetAlpha)
    {
        float start = cg.alpha;
        float t = 0f;
        while (t < hintFadeDuration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, targetAlpha, Mathf.Clamp01(t / hintFadeDuration));
            yield return null;
        }
        cg.alpha = targetAlpha;
        hintFadeCoroutine = null;
    }

    private IEnumerator FadeOutCanvasThenDisable(CanvasGroup cg)
    {
        yield return FadeCanvasGroup(cg, 0f);
        if (hintSprite != null)
            hintSprite.SetActive(false);
    }
}