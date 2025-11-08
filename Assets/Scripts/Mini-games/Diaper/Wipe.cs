using UnityEngine;

public class Wipe : MonoBehaviour
{
    private PoopManager _poopManager;

    void Start()
    {
        _poopManager = FindFirstObjectByType<PoopManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Poop"))
        {
            if (_poopManager != null)
            {
                _poopManager.CurrentPoops--;
            }
            Destroy(other.gameObject);
        }
    }
}
