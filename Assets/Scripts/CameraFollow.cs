using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;            // Player
    public float smoothSpeed = 5f;
    public BoxCollider2D levelBounds;   // The collider representing the level size

    private float camHalfHeight;
    private float camHalfWidth;

    void Start()
    {
        Camera cam = Camera.main;
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    void LateUpdate()
    {
        if (target == null || levelBounds == null)
            return;

        float minX = levelBounds.bounds.min.x + camHalfWidth;
        float maxX = levelBounds.bounds.max.x - camHalfWidth;
        float minY = levelBounds.bounds.min.y + camHalfHeight;
        float maxY = levelBounds.bounds.max.y - camHalfHeight;

        Vector3 targetPos = new Vector3(target.position.x, target.position.y, -10f);

        float clampedX = Mathf.Clamp(targetPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(targetPos.y, minY, maxY);

        Vector3 smoothedPos = Vector3.Lerp(transform.position,
                                           new Vector3(clampedX, clampedY, -10f),
                                           smoothSpeed * Time.deltaTime);

        transform.position = smoothedPos;
    }
}