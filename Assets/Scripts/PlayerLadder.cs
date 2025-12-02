using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLadder : MonoBehaviour
{
    public float climbSpeed = 4f;

    private Rigidbody2D rb;
    private bool isOnLadder = false;
    private float originalGravity;
    private Animator anim;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
        anim = GetComponent<Animator>();

    }

    void Update()
    {
        if (isOnLadder)
        {
            float vertical = Input.GetAxisRaw("Vertical");

            rb.gravityScale = 0f; // no falling
            rb.velocity = new Vector2(rb.velocity.x, vertical * climbSpeed);
        }
        else
        {
            rb.gravityScale = originalGravity;
        }
    }

   void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Ladder"))
    {
        isOnLadder = true;
        rb.velocity = Vector2.zero;
        anim.SetBool("climb", true);   // <-- start climb anim
    }
}

void OnTriggerExit2D(Collider2D other)
{
    if (other.CompareTag("Ladder"))
    {
        isOnLadder = false;
        anim.SetBool("climb", false); 
        anim.SetFloat("climbSpeed", 0f); // <-- stop climb anim
    }
}

}
