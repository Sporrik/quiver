using UnityEngine;

public class CopyRotation : MonoBehaviour
{
    public Transform target = null;
    void Update()
    {
        transform.rotation = target.rotation;
    }
}
