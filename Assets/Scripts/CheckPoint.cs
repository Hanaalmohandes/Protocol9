using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
   void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        Debug.Log("Player entered checkpoint: " + gameObject.name);
        
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.currentCheckpoint = this.gameObject;
            Debug.Log("Checkpoint updated to: " + gameObject.name);
        }
        else
        {
            Debug.LogError("LevelManager not found!");
        }
    }
}
}