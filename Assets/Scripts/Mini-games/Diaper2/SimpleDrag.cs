using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class SimpleDrag : MonoBehaviour
{
    protected Rigidbody rb;

    protected bool _isMouseDragging = false;
    protected bool _isControllerDragging = false;

    [Header("SimpleDrag:")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform DragPlaneYTransform;
    [SerializeField] private MinigameCursor _cursor;

    private GameObject CurrentlyDraggedObject;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnMouseDown()
    {
        if (this.isActiveAndEnabled == false)
            return;

        _isMouseDragging = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

        CurrentlyDraggedObject = gameObject;
    }

    private void OnMouseUp()
    {
        if (this.isActiveAndEnabled == false)
            return;

        _isMouseDragging = false;
        gameObject.GetComponent<MoveToObject>().MoveTo(0);

        if (CurrentlyDraggedObject == gameObject)
            CurrentlyDraggedObject = null;
    }

    private void Update()
    {
        ControllerInput();
        DragObject();
    }

    protected void ControllerInput()
    {
        if (_cursor == null) return;

        if(!_cursor.IsUsed()) return;

        if (_cursor.OnUpEvent())
        {
            gameObject.GetComponent<MoveToObject>().MoveTo(0);

            _isControllerDragging = false;

            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;

            //Debug.Log("OnUp!");
        }

        if (_cursor.OnDownEvent())
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(_cursor.GetPosition().x, _cursor.GetPosition().y, 0f));

            // check if the controller cursor is on the objects
            if (!Physics.Raycast(ray, out hit)) return;

            if (hit.rigidbody != rb) return;

            CurrentlyDraggedObject = gameObject;
            _isControllerDragging = true;

            //Debug.Log("OnDown!");
        }
    }

    // this offset is optional
    protected void DragObject(Vector3 offset = new Vector3())
    {
        const float moveDistance = 0.4f;

        Ray ray = new Ray();

        // check which position the object needs to be dragged to
        if (_isControllerDragging) ray = mainCamera.ScreenPointToRay(_cursor.GetPosition());
        else if (_isMouseDragging) ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        else return;

        Plane dragPlane = new Plane(Vector3.up, new Vector3(0f, DragPlaneYTransform.position.y, 0f));

        // check where on the plane this point hits
        if (!dragPlane.Raycast(ray, out float enter)) return;

        Vector3 hitPoint = ray.GetPoint(enter);

        // move the gameobject to the hitpoint on the plane
        Vector3 targetPosition = new Vector3(hitPoint.x, DragPlaneYTransform.position.y, hitPoint.z) + offset;
        rb.MovePosition(Vector3.Lerp(transform.position, targetPosition, moveDistance));
    }
}
