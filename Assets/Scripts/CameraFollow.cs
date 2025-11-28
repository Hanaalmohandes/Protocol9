using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float CameraSpeed;
    public float minX,maxX;
    public float minY,maxY;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate(){
        Vector2 newCamPosition=Vector2.Lerp(transform.position,target.position,Time.deltaTime*CameraSpeed);
        float clampedX= Mathf.Clamp(newCamPosition.x,minX,maxX);
        float clampedY= Mathf.Clamp(newCamPosition.y,minY,maxY);
        transform.position=new Vector3(clampedX,clampedY,-10);
    }   
}