using UnityEngine;

public class PlayerTransformController : MonoBehaviour
{
    public GameObject form1;
    public GameObject form2;

    private GameObject currentForm;
    public SharedDamageable sharedHealth;

    void Start()
    {
        currentForm = form1;
        currentForm.SetActive(true);
        form2.SetActive(false);

        // به هر فرم، SharedHealth بده
        form1.GetComponent<PlayerForm>().sharedHealth = sharedHealth;
        form2.GetComponent<PlayerForm>().sharedHealth = sharedHealth;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            SwitchForm();
        }
    }

    void SwitchForm()
    {
        Vector3 previousPosition = new Vector3(currentForm.transform.position.x, currentForm.transform.position.y+1, currentForm.transform.position.z);

        currentForm.SetActive(false);
        currentForm = (currentForm == form1) ? form2 : form1;

        currentForm.transform.position = previousPosition;
        currentForm.SetActive(true);
    }

}