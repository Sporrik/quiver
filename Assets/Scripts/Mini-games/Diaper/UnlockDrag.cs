using UnityEngine;

public class UnlockDrag : MonoBehaviour
{
    public LockBones FrontLock;
    public LockBones BackLock;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(FrontLock.IsLocked && BackLock.IsLocked)
        {
            gameObject.GetComponent<Collider>().enabled = true; // Enable collider
            gameObject.GetComponent<DragObject>().enabled = true; // Enable dragging
        }
    }
}
