using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject currentCheckpoint;
    public Transform player;
    
   void Start()
    {
        // Set a default starting checkpoint
        if (currentCheckpoint == null)
        {
            currentCheckpoint = GameObject.Find("Checkpoint"); // or whatever your first checkpoint is named
        }
    }
    
    public void RespawnPlayer()
    {
        if (currentCheckpoint != null && player != null)
        {
            player.position = currentCheckpoint.transform.position;
            Debug.Log("Player respawned at checkpoint!");
        }
        else
        {
            Debug.LogWarning("Cannot respawn - checkpoint or player not set!");
        }
    }
}
  