using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int health = 3;
    public int lives = 3;
    private float flickerTime = 0f;
    private float flickerDuration = 0.1f;
    private SpriteRenderer sr;
    public bool isImmune = false;
    public float immuneDuration = 1.5f;
    private float immunityTimer;
    public static int score = 0;
    public static bool hasHeart = false;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if(isImmune)
        {
            SpriteFlicker(); // Call the flicker method
            immunityTimer += Time.deltaTime;
            if(immunityTimer >= immuneDuration)
            {
                isImmune = false;
                immunityTimer = 0f;
                sr.enabled = true;
            }   
        }
    }
    
    void SpriteFlicker()
    {
        if(flickerTime < flickerDuration)
        {
            flickerTime += Time.deltaTime; 
        }
        else if(flickerTime >= flickerDuration)
        {
            flickerTime = 0;
            sr.enabled = !(sr.enabled);
        }
    }
    
    public void TakeDamage(int damage)
    {
        if(!isImmune)
        {
            health -= damage;
            if(health <= 0 && lives > 0) // Changed from ==0 to <=0
            {
                FindObjectOfType<LevelManager>().RespawnPlayer();
                lives--;
                health = 3;
            }
            else if(lives <= 0 && health <= 0) // Better condition
            {
                Debug.Log("Game Over");
                Destroy(this.gameObject);
            }
            Debug.Log("Player Health: " + health.ToString());
            Debug.Log("Player Lives: " + lives.ToString());
            
            isImmune = true; // Moved inside the if block
            immunityTimer = 0f;
        }
    }
}