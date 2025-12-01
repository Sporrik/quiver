using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class SimpleDrag : MonoBehaviour
{
    private Rigidbody rb;
    public bool IsDragging = false;

    [SerializeField] private GameObject CurrentlyDraggedObject;

    [SerializeField] private Transform DragPlaneYTransform;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private MinigameCursor _cursor;

    private float originalY;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalY = transform.position.y;
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
        gameObject.GetComponent<MoveToObject>().MoveTo(0);

        if (CurrentlyDraggedObject == gameObject)
            CurrentlyDraggedObject = null;
    }

    void Update()
    {
        ControllerSupport();

        if (IsDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // in case controller is used we use my fancy cursor
            if (_cursor.IsUsed()) ray = mainCamera.ScreenPointToRay(_cursor.GetPosition());

            Plane dragPlane = new Plane(Vector3.up, new Vector3(0f, DragPlaneYTransform.position.y, 0f));
            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);

                Vector3 targetPosition = new Vector3(hitPoint.x, DragPlaneYTransform.position.y, hitPoint.z);
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
