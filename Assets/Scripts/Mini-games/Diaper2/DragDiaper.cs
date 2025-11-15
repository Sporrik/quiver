using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class DragDiaper : MonoBehaviour
{
    private Rigidbody rb;
    public bool IsDragging = false;

    [SerializeField] private GameObject CurrentlyDraggedObject;
    [SerializeField] private Transform DragPlaneYTransform;
    [SerializeField] private Camera mainCamera;

    [SerializeField] private Transform meshTransform; // Reference to the mesh's transform
    [SerializeField] private PoopManager poopManager; // Reference to the PoopManager

    private float originalY;
    private Vector3 meshOffset; // Offset between the rig and the mesh

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalY = transform.position.y;

        // Calculate the offset between the rig's pivot and the mesh's pivot
        meshOffset = transform.position - meshTransform.position;
    }

    void OnMouseDown()
    {
        if (this.isActiveAndEnabled == false)
            return;

        IsDragging = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

        CurrentlyDraggedObject = gameObject;
    }

    public void OnMouseUp()
    {
        if (this.isActiveAndEnabled == false)
            return;

        IsDragging = false;

        if (gameObject.GetComponent<MoveToObject>() != null)
            gameObject.GetComponent<MoveToObject>().MoveTo(0);

        if (CurrentlyDraggedObject == gameObject)
            CurrentlyDraggedObject = null;
    }

    void Update()
    {
        if (IsDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane dragPlane = new Plane(Vector3.up, new Vector3(0f, DragPlaneYTransform.position.y, 0f));
            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);

                // Adjust the target position to account for the mesh's offset
                Vector3 targetPosition = new Vector3(hitPoint.x, DragPlaneYTransform.position.y, hitPoint.z) + meshOffset;
                rb.MovePosition(Vector3.Lerp(transform.position, targetPosition, 0.4f));
            }
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name == "DiaperGoal")
        {
            IsDragging = false;
            gameObject.GetComponent<MoveToObject>().MoveTo(1);
            gameObject.GetComponent<DiaperChangingBehavior>().enabled = true;
            poopManager.CleanDiaperEquipped = true;
            Destroy(this);
        }
    }
}
