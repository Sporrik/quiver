using UnityEngine;

public class FlipBaby : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private Vector3 rotationA = new Vector3(0, 0, 0);      // Starting rotation (Euler angles)
    [SerializeField] private Vector3 rotationB = new Vector3(0, 180, 0);    // Target rotation (Euler angles)
    [SerializeField] private float rotationSpeed = 2f;                      // Speed of rotation lerp

    [Header("Click Settings:")]
    [SerializeField] private Camera _camera;            // Reference to the _camera
    [SerializeField] private Collider triggerCollider;  // Collider to detect clicks
    [SerializeField] private MinigameCursor _cursor;    // the cursor with controller support

    private bool isRotating = false;    // Flag to determine if rotation is in progress
    private Quaternion targetRotation;  // Target rotation
    private Quaternion startRotation;   // Starting rotation
    private float lerpProgress = 0f;    // Progress of the lerp
    public bool isFlipped = false;      // Tracks the current rotation state
    private PoopManager _poopManager;

    void Start()
    {
        // Initialize rotations
        startRotation = Quaternion.Euler(rotationA);
        targetRotation = startRotation;
        transform.rotation = startRotation;
        _poopManager = FindFirstObjectByType<PoopManager>();
    }

    void Update()
    {
        ControllerSupport();

        if (isRotating)
        {
            LerpRotation();
        }
    }
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _poopManager.ChangeMouseCursor(2); // Change cursor to clicking state   
        }
    }
    private void OnMouseExit()
    {
        _poopManager.ChangeMouseCursor(0); // Change cursor to default state
    }
    void OnMouseDown()
    {
        OnClickBaby(Input.mousePosition);
    }

    private void ControllerSupport()
    {
        if(_cursor.OnDownEvent())
        {
            OnClickBaby(_cursor.GetPosition());
        }
    }

    private void LerpRotation()
    {
        // Smoothly interpolate rotation
        lerpProgress += Time.deltaTime * rotationSpeed;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpProgress);

        // Stop lerping when the rotation is close to the target
        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            transform.rotation = targetRotation; // Snap to the target rotation
            isRotating = false;
        }
    }

    private void OnClickBaby(Vector3 screenPosition)
    {
        // Detect if the mouse is over the trigger collider

        Ray ray = _camera.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider == triggerCollider)
        {
            // Toggle the target rotation
            isFlipped = !isFlipped;
            targetRotation = isFlipped ? Quaternion.Euler(rotationB) : Quaternion.Euler(rotationA);

            // Start the rotation
            isRotating = true;
            lerpProgress = 0f; // Reset lerp progress
        }
    }
}
