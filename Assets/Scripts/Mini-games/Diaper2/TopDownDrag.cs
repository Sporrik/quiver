using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class TopDownDrag : MonoBehaviour
{
    private Rigidbody rb;
    public bool IsDragging = false;

    public GameObject CurrentlyDraggedObject;

    public float DragPlaneY = 2f; // Hover height

    [SerializeField] private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        if(this.isActiveAndEnabled == false)
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
        rb.useGravity = true;

        if (CurrentlyDraggedObject == gameObject)
            CurrentlyDraggedObject = null;

    }

    void Update()
    {
        if (IsDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane dragPlane = new Plane(Vector3.up, new Vector3(0f, DragPlaneY, 0f));
            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);

                Vector3 targetPosition = new Vector3(hitPoint.x, DragPlaneY, hitPoint.z);
                rb.MovePosition(Vector3.Lerp(transform.position, targetPosition, 0.4f));
            }
        }
    }
}
