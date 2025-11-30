using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CrateSearch : MonoBehaviour
{
    public bool containsAccessCard = false;
    public GameObject accessCardPrefab;
    public Transform spawnPoint;
    private bool playerNear = false;

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            SearchCrate();
        }
    }

    void SearchCrate()
    {
PlayerController pm = FindObjectOfType<PlayerController>();
pm.PlaySearchAnimation();
        if (containsAccessCard)
        {
            Instantiate(accessCardPrefab, spawnPoint.position, Quaternion.identity);
            containsAccessCard = false;
        }
        // Optional: play sound, animation, or show "Empty"
        Debug.Log("Crate searched.");
    }

    private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("Player"))
    {
        playerNear = true;
    }
}

private void OnTriggerExit2D(Collider2D collision)
{
    if (collision.CompareTag("Player"))
    {
        playerNear = false;
    }
}
}