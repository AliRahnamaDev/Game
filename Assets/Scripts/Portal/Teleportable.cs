using UnityEngine;

public class Teleportable : MonoBehaviour
{
    [HideInInspector] public bool justTeleported = false;

    private void OnEnable()
    {
        justTeleported = false;
    }
}