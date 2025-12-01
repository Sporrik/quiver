using UnityEngine;

public class ActivateParticle : MonoBehaviour
{
    [SerializeField] private GameObject _particle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name=="ItemEater")
        {
            _particle.SetActive(true);
        }
    }
}
