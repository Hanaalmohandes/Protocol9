using UnityEngine;
using UnityEngine.Events;

public class SearchlightDetector : MonoBehaviour
{
    public string playerTag = "Player";
    public bool detectOnEnter = true;
    public bool detectOnStay = false;
    public bool detectOnExit = false;

    // If true the detector will apply damage before (or in addition to) respawning the player.
    // Note: this default is true so detectors that cause respawn will also apply damage unless
    // the Inspector explicitly turns it off.
    public bool damagePlayer = true;
    public int damageAmount = 1;
    public bool respawnOnDetect = true;

    public UnityEvent onPlayerDetected;

    // When false the detector is considered 'off' and will not apply damage or respawn.
    // You can toggle this from the Inspector or control it from another script.
    public bool beamActive = true;
    // Optional collider reference for the visible/physical beam. When set, the
    // detector uses `beamCollider.enabled` to determine whether detections are
    // active. If left null, `beamActive` is used instead.
    public Collider2D beamCollider;
    // Cover detection settings: when the player is crouching, we'll do a
    // small CircleCast between detector and player and ignore detection if a
    // cover-layer object is found. Use the Inspector to select the layer(s).
    public float coverCheckRadius = 0.2f;
    public LayerMask coverLayer;
    // Optional tag name fallback for cover objects. If this tag is not defined
    // in the project's Tag Manager, the code will warn once and not use the tag.
    public string coverTag = "Cover";

    // Internal: only warn once if the configured coverTag isn't defined.
    private bool coverTagWarningLogged = false;

    private PlayerStats playerStats;
    private LevelManager levelManager;

    private void Awake()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        levelManager = FindObjectOfType<LevelManager>();
        // If the beamCollider wasn't assigned in the Inspector, try to find a
        // Collider2D on the same GameObject.
        if (beamCollider == null)
            beamCollider = GetComponent<Collider2D>();

        // If coverLayer wasn't configured, try to default to a layer named "Cover".
        if (coverLayer.value == 0)
        {
            coverLayer = LayerMask.GetMask("Cover");
            if (coverLayer.value != 0)
                Debug.Log($"SearchlightDetector: defaulted coverLayer to layer 'Cover' for detector '{name}'.");
            else
                Debug.LogWarning($"SearchlightDetector: coverLayer is not set on detector '{name}'. Crates will only block detection if player is crouching AND coverLayer or tag 'Cover' is configured.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (detectOnEnter)
        {
            HandleDetection(other);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (detectOnStay)
        {
            HandleDetection(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (detectOnExit)
        {
            HandleDetection(other);
        }
    }

    private void HandleDetection(Collider2D other)
    {
        // If a beamCollider is present, use its enabled state to determine
        // whether the beam is active; otherwise fall back to the Inspector
        // `beamActive` flag. This ensures turning the collider off stops
        // damage/respawn automatically.
        bool isActive = (beamCollider != null) ? beamCollider.enabled : beamActive;
        if (!isActive)
            return;
        if (!other.CompareTag(playerTag))
            return;

        // Resolve the player's controller (allow for collider on child objects)
        var playerController = other.GetComponentInParent<PlayerController>();
        if (playerController != null && playerController.IsCrouching)
        {
            // Perform a small CircleCast between detector and player to detect
            // intervening cover objects (crates). If found, ignore detection.
            Vector2 origin = (Vector2)transform.position;
            Vector2 target = (Vector2)other.transform.position;
            Vector2 dir = target - origin;
            float distance = dir.magnitude;
            if (distance > 0f)
            {
                RaycastHit2D[] hits = Physics2D.CircleCastAll(origin, coverCheckRadius, dir.normalized, distance + 0.01f);
                foreach (var h in hits)
                {
                    if (h.collider == null)
                        continue;
                    var go = h.collider.gameObject;
                    // Ignore the player's own collider(s)
                    if (go == other.gameObject || go.transform.IsChildOf(other.transform))
                        continue;
                    // If the collider's layer is included in the coverLayer mask,
                    // treat it as cover and ignore the detection. Also accept a
                    // fallback check: objects tagged 'Cover' are treated as cover
                    // when the LayerMask hasn't been configured in the Inspector.
                    bool isCoverByLayer = (coverLayer.value & (1 << go.layer)) != 0;
                    bool isCoverByTag = false;
                    if (!string.IsNullOrEmpty(coverTag))
                    {
                        try
                        {
                            isCoverByTag = go.CompareTag(coverTag);
                        }
                        catch
                        {
                            if (!coverTagWarningLogged)
                            {
                                Debug.LogWarning($"SearchlightDetector: tag '{coverTag}' is not defined in Tag Manager. Set up the tag or change SearchlightDetector.coverTag. This warning will only appear once.");
                                coverTagWarningLogged = true;
                            }
                            isCoverByTag = false;
                        }
                    }

                    if (isCoverByLayer || isCoverByTag)
                    {
                        Debug.Log($"SearchlightDetector: cover detected between detector '{name}' and player via object '{go.name}'. Ignoring detection.");
                        return;
                    }
                }
            }
        }

        

        // Apply damage if needed. If we're applying damage, let PlayerStats
        // handle death/respawn (it calls LevelManager.RespawnPlayer when health<=0).
        // Only call RespawnPlayer directly when damage is NOT applied but
        // a forced respawn is desired (respawnOnDetect == true).
        if (damagePlayer && playerStats != null)
        {
            playerStats.TakeDamage(damageAmount);
        }
        else if (respawnOnDetect && levelManager != null)
        {
            levelManager.RespawnPlayer();
        }

        // Trigger extra events (sound, alarm light, UI flash, etc.)
        onPlayerDetected?.Invoke();
    }
}