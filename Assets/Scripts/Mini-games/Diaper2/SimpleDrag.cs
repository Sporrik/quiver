using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class SimpleDrag : MonoBehaviour
{
    private Rigidbody rb;
    public bool _isMouseDragging = false;
    private bool _isControllerDragging = false;

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

        _isMouseDragging = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

        CurrentlyDraggedObject = gameObject;
    }

    public void OnMouseUp()
    {
        if (this.isActiveAndEnabled == false)
            return;

        _isMouseDragging = false;
        gameObject.GetComponent<MoveToObject>().MoveTo(0);

        if (CurrentlyDraggedObject == gameObject)
            CurrentlyDraggedObject = null;
    }

    void Update()
    {
        if (DraggedByController()) return;

        if (_isMouseDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            Plane dragPlane = new Plane(Vector3.up, new Vector3(0f, DragPlaneYTransform.position.y, 0f));
            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);

                Vector3 targetPosition = new Vector3(hitPoint.x, DragPlaneYTransform.position.y, hitPoint.z);
                rb.MovePosition(Vector3.Lerp(transform.position, targetPosition, 0.4f));
            }
        }
    }
    private bool DraggedByController()
    {
        if (_cursor == null) return false;

        if(!_cursor.IsUsed()) return false;

        if (_cursor.OnUpEvent())
        {
            gameObject.GetComponent<MoveToObject>().MoveTo(0);
            return false;
        }

        if (!_cursor.IsPressed()) return false;

        RaycastHit hit;

        Ray ray = ray = mainCamera.ScreenPointToRay(new Vector3(_cursor.GetPosition().x, _cursor.GetPosition().y, 0f));

        if(!Physics.Raycast(ray, out hit)) return false;

        if(hit.rigidbody != rb) return false;

        Plane dragPlane = new Plane(Vector3.up, new Vector3(0f, DragPlaneYTransform.position.y, 0f));

        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            Vector3 targetPosition = new Vector3(hitPoint.x, DragPlaneYTransform.position.y, hitPoint.z);
            rb.MovePosition(Vector3.Lerp(transform.position, targetPosition, 0.4f));
        }

        return true;
    }
}
