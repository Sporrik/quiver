using System.Collections.Generic;
using UnityEngine;

public class LockBones : MonoBehaviour
{
    [SerializeField] private List<Transform> Bones; // List of bones to lock
    [SerializeField] private GameObject TargetObject; // Target object to compare position

    public bool IsLocked = false; // Flag to ensure locking happens only once
    private void LockBonePositions()
    {
        foreach (var bone in Bones)
        {
            if (bone != null)
            {
                // Freeze the rigidbody if it exists
                Rigidbody rb = bone.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.constraints = RigidbodyConstraints.FreezeAll; // Freeze all movement and rotation
                }
            }
        }

        Debug.Log("Bones locked at their current positions.");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == TargetObject && !IsLocked)
        {
            LockBonePositions();
            IsLocked = true; // Prevent further locking
            gameObject.GetComponent<DragObject>().enabled = false; // Disable dragging
            Debug.Log("Object reached target position. Bones locked.");
        }
    }

    public void UnlockBones()
    {
        Debug.Log("Unlocking bones...");

        foreach (var bone in Bones)
        {
            if (bone != null)
            {
                // Unfreeze the rigidbody if it exists
                Rigidbody rb = bone.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.constraints = RigidbodyConstraints.None; // Unfreeze all movement and rotation
                }
            }
        }
        //IsLocked = false; // Allow locking again if needed
        Debug.Log("Bones unlocked.");
    }
}
