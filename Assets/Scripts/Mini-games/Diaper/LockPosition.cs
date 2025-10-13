using UnityEngine;

public class LockPosition : MonoBehaviour
{
    public GameObject targetObject; // The object to compare position with
    public GameObject CP_Front;
    public GameObject CP_Back;

    public bool IsLocked = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == targetObject)
        {
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                gameObject.GetComponent<Collider>().enabled = false;
                gameObject.GetComponent<DragObject>().enabled = false;

                CopyRotation copyRotation = gameObject.GetComponent<CopyRotation>();
                if (copyRotation != null)
                {
                    copyRotation.enabled = true;
                    Debug.Log($"{gameObject.GetComponent<CopyRotation>().enabled}");
                }
                else
                {
                    Debug.LogError("CopyRotation component is missing on this GameObject.");
                }

                rb.isKinematic = true;
                CP_Front.SetActive(true);
                CP_Back.SetActive(true);

                Debug.Log("Position locked.");
                IsLocked = true;
            }
        }
    }
}
