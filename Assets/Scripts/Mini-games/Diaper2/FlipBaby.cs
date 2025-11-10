using UnityEngine;

public class FlipBaby : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private Vector3 rotationA = new Vector3(0, 0, 0); // Starting rotation (Euler angles)
    [SerializeField] private Vector3 rotationB = new Vector3(0, 180, 0); // Target rotation (Euler angles)
    [SerializeField] private float rotationSpeed = 2f; // Speed of rotation lerp

    [Header("Drag Settings")]
    [SerializeField] private new Camera camera; // Reference to the camera
    [SerializeField] private Collider triggerCollider; // Collider to detect dragging

    private bool isDragging = false;
    private bool isRotatingToB = false; // Flag to determine rotation direction
    private Quaternion targetRotation; // Target rotation
    private Quaternion startRotation; // Starting rotation
    private float lerpProgress = 0f; // Progress of the lerp

    private Vector3 lastMousePosition;

    void Start()
    {
        // Initialize rotations
        startRotation = Quaternion.Euler(rotationA);
        targetRotation = Quaternion.Euler(rotationB);
    }

    void Update()
    {
        if (isDragging)
        {
            HandleDragging();
        }

        if (isRotatingToB)
        {
            LerpRotation();
        }
    }

    void OnMouseDown()
    {
        // Detect if the mouse is over the trigger collider
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider == triggerCollider)
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        // Start lerping rotation when dragging stops
        isRotatingToB = true;
        lerpProgress = 0f; // Reset lerp progress
    }

    private void HandleDragging()
    {
        Vector3 currentMousePosition = Input.mousePosition;
        Vector3 dragDelta = currentMousePosition - lastMousePosition;

        // Determine rotation direction based on drag delta
        if (dragDelta.x > 0)
        {
            targetRotation = Quaternion.Euler(rotationB); // Rotate to B
        }
        else if (dragDelta.x < 0)
        {
            targetRotation = Quaternion.Euler(rotationA); // Rotate to A
        }

        lastMousePosition = currentMousePosition;
    }

    private void LerpRotation()
    {
        // Smoothly interpolate rotation
        lerpProgress += Time.deltaTime * rotationSpeed;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpProgress);

        // Stop lerping when the rotation is close to the target
        if (Quaternion.Angle(transform.localRotation, targetRotation) < 0.1f)
        {
            isRotatingToB = false;
        }
    }
}
