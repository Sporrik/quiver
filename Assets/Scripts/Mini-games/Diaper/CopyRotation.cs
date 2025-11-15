using UnityEngine;

public class CopyRotation : MonoBehaviour
{
    [SerializeField] private Transform target = null;
    void Update()
    {
        transform.rotation = target.rotation;
    }
}
