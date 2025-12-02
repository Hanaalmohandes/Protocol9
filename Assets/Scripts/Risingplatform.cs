using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Risingplatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    public float speed;
    private bool AtoB;
    private Vector3 targetPosition;

    void FixedUpdate()   // MOVING PLATFORM MUST USE FIXEDUPDATE
    {
        if (AtoB)
        {
            targetPosition = pointA.position;
        }
        else
        {
            targetPosition = pointB.position;
        }

        Vector3 newPosition = transform.position;
        newPosition.y = Mathf.MoveTowards(transform.position.y, targetPosition.y, speed * Time.fixedDeltaTime);
        transform.position = newPosition;

        if (Mathf.Abs(transform.position.y - targetPosition.y) < 0.1f)
        {
            AtoB = !AtoB;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
