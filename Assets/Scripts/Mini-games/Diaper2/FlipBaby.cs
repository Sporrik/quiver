using UnityEngine;

public class FlipBaby : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private Vector3 rotationA = new Vector3(0, 0, 0); // Starting rotation (Euler angles)
    [SerializeField] private Vector3 rotationB = new Vector3(0, 180, 0); // Target rotation (Euler angles)
    [SerializeField] private float rotationSpeed = 2f; // Speed of rotation lerp

    [Header("Click Settings")]
    public new Camera camera; // Reference to the camera
    [SerializeField] private Collider triggerCollider; // Collider to detect clicks

    private bool isRotating = false; // Flag to determine if rotation is in progress
    private Quaternion targetRotation; // Target rotation
    private Quaternion startRotation; // Starting rotation
    private float lerpProgress = 0f; // Progress of the lerp
    public bool isFlipped = false; // Tracks the current rotation state

    void Start()
    {
        // Initialize rotations
        startRotation = Quaternion.Euler(rotationA);
        targetRotation = startRotation;
        transform.rotation = startRotation;
    }

    void Update()
    {
        if (isRotating)
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
            // Toggle the target rotation
            isFlipped = !isFlipped;
            targetRotation = isFlipped ? Quaternion.Euler(rotationB) : Quaternion.Euler(rotationA);

            // Start the rotation
            isRotating = true;
            lerpProgress = 0f; // Reset lerp progress
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
}
