using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class TopDownDrag : MonoBehaviour
{
    private Rigidbody rb;
    public bool IsDragging = false;

    [SerializeField] private GameObject CurrentlyDraggedObject;

    [SerializeField] private float DragPlaneY = 2f; // Hover height

    [SerializeField] private Camera mainCamera;

    [SerializeField] private MinigameCursor _cursor;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //mainCamera = Camera.main;

        DragPlaneY = transform.position.y + DragPlaneY;
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
        ControllerSupport();

        if (IsDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // still needs fixing, weeeeeee
            if(_cursor.IsUsed()) ray = mainCamera.ViewportPointToRay(_cursor.GetPosition());

            Plane dragPlane = new Plane(Vector3.up, new Vector3(0f, DragPlaneY, 0f));
            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);

                Vector3 targetPosition = new Vector3(hitPoint.x, DragPlaneY, hitPoint.z);
                rb.MovePosition(Vector3.Lerp(transform.position, targetPosition, 0.4f));
            }
        }
    }

    private void ControllerSupport()
    {
        if (_cursor == null) return;

        if (_cursor.OnDownEvent())
        {
            OnMouseDown();
            Debug.Log("Down event!");
        }

        if (_cursor.OnUpEvent())
        {
            OnMouseUp();
            Debug.Log("Up event!");
        }
    }
}
