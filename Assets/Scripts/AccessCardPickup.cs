using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessCardPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Playerinventory inventory = other.GetComponent<Playerinventory>();
            if (inventory != null)
            {
                inventory.hasAccessCard = true;
                Destroy(gameObject);
            }
        }
    }
}