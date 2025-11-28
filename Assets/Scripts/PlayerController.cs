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
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
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
anim.SetFloat("Speed",Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x));
anim.SetFloat("Height", GetComponent<Rigidbody2D>().velocity.y);
anim.SetBool("Grounded",Grounded);
    }
    void Jump(){
        GetComponent<Rigidbody2D>().velocity= new Vector2(GetComponent<Rigidbody2D>().velocity.x, jumpHeight);
    }
    void FixedUpdate(){
        Grounded=Physics2D.OverlapCircle(GroundCheck.position,GroundCheckRadius,WhatIsGround);
    }
}
