using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform cameraTransform;
    public float parallaxFactor = 0.5f;
    public float activateDistance = 30f;

    private Vector3 startPosition;
    private Vector3 startCameraPosition;
    private bool initialized = false;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (!initialized)
        {
            float distance = Vector3.Distance(transform.position, cameraTransform.position);
            if (distance < activateDistance)
            {
                startPosition = transform.position;
                startCameraPosition = cameraTransform.position;
                initialized = true;
            }
            else return;
        }

        Vector3 delta = cameraTransform.position - startCameraPosition;
        transform.position = startPosition + new Vector3(delta.x * parallaxFactor, 0f, 0f);
    }
}