using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float jumpHeight;
    public KeyCode spaceBar;
    public KeyCode L;
    public KeyCode R;
    public Transform GroundCheck;
    public float GroundCheckRadius;
    public LayerMask WhatIsGround;
    public bool Grounded;
    // Keep track of the last known safe position (when grounded)
    private Vector2 lastSafePosition;
    // If player's Y drops below this value they'll be teleported back to lastSafePosition
    public float fallRespawnY = -10f;
    // If true, teleport back to lastSafePosition when falling below fallRespawnY
    public bool teleportOnFall = true;
    // Small upward offset applied when teleporting back to avoid immediately re-triggering fall
    public float respawnOffsetY = 0.5f;
    // LevelManager reference: if present we'll use it to respawn the player
    private LevelManager levelManager;
    
    // Crouch input: assignable in Inspector (e.g. LeftControl, S, DownArrow)
    public KeyCode crouchKey = KeyCode.LeftControl;
    // Interaction key (assignable in Inspector)
    public KeyCode interactKey = KeyCode.E;
    // Optional hint GameObject to show when an interactable is available (assign in Inspector)
    public GameObject interactHint;
    // Current crouch state (driven by input or UI). Use `SetCrouch` to control from UI.
    private bool isCrouchingInput = false;
    // Public read-only accessor so other components can query crouch state.
    public bool IsCrouching { get { return isCrouchingInput; } }
    // Current interactable (set by interactable objects when player enters their trigger)
    [HideInInspector]
    public CrateSearch currentInteractable;

    // Called by interactable objects to register themselves. Shows hint if assigned.
    public void SetCurrentInteractable(CrateSearch cs)
    {
        currentInteractable = cs;
        if (interactHint != null)
            interactHint.SetActive(cs != null);
    }

    public void ClearCurrentInteractable(CrateSearch cs)
    {
        if (currentInteractable == cs)
        {
            currentInteractable = null;
            if (interactHint != null)
                interactHint.SetActive(false);
        }
    }

    // Call this from UI Button OnClick to trigger interaction (useful for touch/UI)
    public void InteractPressed()
    {
        if (isSearching) return;
        if (currentInteractable != null)
        {
            PlaySearchAnimation();
            currentInteractable.SearchCrate();
        }
    }

    // Search animation settings (configurable in Inspector)
    public string searchParam = "isSearching";
    public float searchDuration = 0.6f;
    private Animator anim;
    public bool isSearching = false; // when true, player input for movement is ignored
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            // Try to find Animator in children if not on the same GameObject
            anim = GetComponentInChildren<Animator>();
            if (anim != null)
                Debug.Log($"PlayerController: found Animator in child on '{name}'.");
            else
                Debug.LogWarning($"PlayerController: no Animator found on '{name}' or its children. Assign one in the Inspector.");
        }

        // Initialize lastSafePosition to the starting position so we always have
        // a reasonable fallback if the player falls before ever touching ground.
        lastSafePosition = transform.position;

        // Try to find a LevelManager to handle respawning if present
        levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
            Debug.Log($"PlayerController: found LevelManager '{levelManager.name}' and will use it for fall respawn.");
        if (GroundCheck == null)
            Debug.LogWarning($"PlayerController: GroundCheck Transform is not assigned on '{name}'. Ground detection and fall-respawn may not work correctly.");
    }

    // Update is called once per frame
    void Update()
    {
        // Update crouch from keyboard input OR from Animator parameter if animation drives crouch
        bool inputCrouch = Input.GetKey(crouchKey);
        bool animCrouch = false;
        if (anim != null && AnimatorHasParameter(anim, "isCrouching"))
        {
            try { animCrouch = anim.GetBool("isSearching") ? false : anim.GetBool("isCrouching"); } catch { animCrouch = false; }
        }
        isCrouchingInput = inputCrouch || animCrouch;
        if (anim != null)
            anim.SetBool("isCrouching", isCrouchingInput);
        // When searching, block player movement input
        if (!isSearching)
        {
            // Interaction input handled here so the button can be assigned in the Inspector
            if (Input.GetKeyDown(interactKey) && currentInteractable != null)
            {
                // Trigger search animation and delegate the crate search logic
                PlaySearchAnimation();
                currentInteractable.SearchCrate();
            }
            if(Input.GetKey(spaceBar)&&Grounded)  {
                Jump();
            }

            if (Input.GetKey(L)) {
                GetComponent<Rigidbody2D>().velocity = new Vector2(-moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (Input.GetKey(R)) {
                GetComponent<Rigidbody2D>().velocity = new Vector2(moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
                GetComponent<SpriteRenderer>().flipX = false;
            }
        }
        if (anim != null)
        {
            anim.SetFloat("Speed",Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x));
            anim.SetFloat("Height", GetComponent<Rigidbody2D>().velocity.y);
            anim.SetBool("Grounded",Grounded);
        }

        // If the player falls below the allowed Y, either ask LevelManager to
        // respawn (preferred) or fall back to teleporting to lastSafePosition.
        if (teleportOnFall && transform.position.y < fallRespawnY)
        {
            if (levelManager != null)
            {
                Debug.LogWarning($"PlayerController: player Y={transform.position.y} fell below threshold {fallRespawnY}. Calling LevelManager.RespawnPlayer().");
                levelManager.RespawnPlayer();
            }
            else
            {
                Vector2 spawnPos = new Vector2(lastSafePosition.x, lastSafePosition.y + respawnOffsetY);
                Debug.LogWarning($"PlayerController: player Y={transform.position.y} fell below threshold {fallRespawnY}. Teleporting to last safe position {lastSafePosition} -> spawnPos {spawnPos}.");
                var rb = GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                    rb.position = spawnPos;
                    rb.Sleep();
                    rb.WakeUp();
                }
                transform.position = spawnPos;
            }
        }
    }
    void Jump(){
        GetComponent<Rigidbody2D>().velocity= new Vector2(GetComponent<Rigidbody2D>().velocity.x, jumpHeight);
    }
    void FixedUpdate(){
        Grounded=Physics2D.OverlapCircle(GroundCheck.position,GroundCheckRadius,WhatIsGround);
        // Update lastSafePosition when grounded so we can restore if the player falls
        if (Grounded)
        {
            lastSafePosition = transform.position;
        }
    }
    public void PlaySearchAnimation()
    {
        if (anim == null)
        {
            Debug.LogWarning($"PlaySearchAnimation called but no Animator found on PlayerController '{name}'.");
            return;
        }

        if (!AnimatorHasParameter(anim, searchParam))
        {
            Debug.LogWarning($"Animator on PlayerController '{name}' does not contain parameter '{searchParam}'. Add it or update PlayerController.searchParam.");
            return;
        }

        isSearching = true;
        anim.SetBool(searchParam, true);
        StartCoroutine(StopSearch());
    }

    private IEnumerator StopSearch()
    {
        yield return new WaitForSeconds(searchDuration);
        if (anim != null && AnimatorHasParameter(anim, searchParam))
            anim.SetBool(searchParam, false);
        isSearching = false;
    }

    // Helper: check if animator contains a parameter with the given name.
    private bool AnimatorHasParameter(Animator animator, string paramName)
    {
        if (animator == null) return false;
        foreach (var p in animator.parameters)
            if (p.name == paramName) return true;
        return false;
    }
}
