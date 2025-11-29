using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SearchlightBeamToggle : MonoBehaviour
{
    public SpriteRenderer beamRenderer;
    public float interval = 3f;
    private bool isOn = true;
    // Optional references so toggling the visual beam also disables the
    // physical trigger and notifies the detector logic.
    public Collider2D beamCollider;
    private SearchlightDetector detector;

    void Start()
    {
        if (beamRenderer == null)
            beamRenderer = GetComponent<SpriteRenderer>();

        if (beamCollider == null)
            beamCollider = GetComponent<Collider2D>();

        detector = GetComponent<SearchlightDetector>();

        StartCoroutine(ToggleRoutine());
    }

    private System.Collections.IEnumerator ToggleRoutine()
    {
        while (true)
        {
            ToggleBeam();
            yield return new WaitForSeconds(interval);
        }
    }

    private void ToggleBeam()
    {
        isOn = !isOn;
        beamRenderer.enabled = isOn;
        // Keep the collider in sync so triggers stop firing when the beam
        // is visually off.
        if (beamCollider != null)
            beamCollider.enabled = isOn;

        // Also set the detector state so it will ignore detections if it's
        // using a separate collider or manual flag.
        if (detector != null)
        {
            if (detector.beamCollider != null)
                detector.beamCollider.enabled = isOn;
            else
                detector.beamActive = isOn;
        }
    }

    public bool IsBeamActive()
    {
        return isOn;
    }
}