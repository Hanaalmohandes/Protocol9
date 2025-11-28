using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private Camera cam;
    public float targetZoom=3;
    public float zoomSpeed=3;
    private float scrollData;
    // Start is called before the first frame update
    void Start()
    {
        cam=GetComponent<Camera>();
        targetZoom=cam.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        scrollData=Input.GetAxis("Mouse ScrollWheel");
        targetZoom= targetZoom-scrollData;
        targetZoom= Mathf.Clamp(targetZoom,3,6);
        cam.orthographicSize= Mathf.Lerp(cam.orthographicSize,targetZoom,zoomSpeed*Time.deltaTime);
    }
}