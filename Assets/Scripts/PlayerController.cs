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
    
    // Crouch input: assignable in Inspector (e.g. LeftControl, S, DownArrow)
    public KeyCode crouchKey = KeyCode.LeftControl;
    // Current crouch state (driven by input or UI). Use `SetCrouch` to control from UI.
    private bool isCrouchingInput = false;
    // Public read-only accessor so other components can query crouch state.
    public bool IsCrouching { get { return isCrouchingInput; } }

    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Update crouch from keyboard input
        isCrouchingInput = Input.GetKey(crouchKey);
        if (anim != null)
            anim.SetBool("isCrouching", isCrouchingInput);

        if(Input.GetKey(spaceBar)&&Grounded)  {
            Jump();
        }  
    if (Input.GetKey(L)) {
    GetComponent<Rigidbody2D>().velocity = new Vector2(-moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
    GetComponent<SpriteRenderer>().flipX = true;
}
if (Input.GetKey(R)) {
    GetComponent<Rigidbody2D>().velocity = new Vector2(moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
    GetComponent<SpriteRenderer>().flipX = false;
}
        if (anim != null)
        {
            anim.SetFloat("Speed",Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x));
            anim.SetFloat("Height", GetComponent<Rigidbody2D>().velocity.y);
            anim.SetBool("Grounded",Grounded);
        }
    }
    void Jump(){
        GetComponent<Rigidbody2D>().velocity= new Vector2(GetComponent<Rigidbody2D>().velocity.x, jumpHeight);
    }
    void FixedUpdate(){
        Grounded=Physics2D.OverlapCircle(GroundCheck.position,GroundCheckRadius,WhatIsGround);
    }
    public void PlaySearchAnimation()
{
    anim.SetBool("isSearching", true);
    StartCoroutine(StopSearch());
}

private IEnumerator StopSearch()
{
    yield return new WaitForSeconds(0.6f); // length of animation
    anim.SetBool("isSearching", false);
}
}
