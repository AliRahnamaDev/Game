using UnityEngine;

public class KeyHandler : MonoBehaviour
{
    public bool hasKey = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Key"))
        {
            hasKey = true;
            Destroy(collision.gameObject); // حذف کلید پس از برداشتن
            Debug.Log($"{gameObject.name} picked up a key.");
        }

        if (collision.CompareTag("Door") && hasKey)
        {
            Destroy(collision.gameObject);
            Debug.Log($"{gameObject.name} opened the door.");
        }
    }
}