using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform cameraTransform;
    public float parallaxFactor = 0.5f;
    private Vector3 previousCameraPosition;

    void Start()
    {
        previousCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - previousCameraPosition;
        
        transform.position += new Vector3(deltaMovement.x * parallaxFactor, 0f, 0f);

        previousCameraPosition = cameraTransform.position;
    }
}