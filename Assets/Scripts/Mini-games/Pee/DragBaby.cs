using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DragBaby : MonoBehaviour
{
    [Header("Drag Settings")]
    public float dragSpeed = 10f; // Speed multiplier for dragging
    public float damping = 5f; // Damping to smooth the movement

    [Header("Position Limits")]
    public float minX = -5f; // Minimum X position
    public float maxX = 5f; // Maximum X position

    private Camera mainCamera;
    private Rigidbody rb;
    private bool isDragging = false;
    private Vector3 targetPosition;

    void Start()
    {
        // Cache the main camera and Rigidbody reference
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false; // Rigidbody must not be kinematic for physics interactions
    }

    void Update()
    {
        if (isDragging)
        {
            // Update the target position based on the mouse position
            Vector3 mousePosition = GetMouseWorldPosition();

            // Clamp the X position within the specified limits
            targetPosition.x = Mathf.Clamp(mousePosition.x, minX, maxX);
        }
    }

    void FixedUpdate()
    {
        if (isDragging)
        {
            // Calculate the direction to the target position
            Vector3 direction = (targetPosition - transform.position);

            // Smoothly move the object toward the target position
            Vector3 velocity = direction * dragSpeed;

            // Apply the velocity to the Rigidbody
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, velocity, Time.fixedDeltaTime * damping);
        }
    }

    void OnMouseDown()
    {
        // Start dragging and set the initial target position
        isDragging = true;
        targetPosition = transform.position;

        // Stop any existing velocity to prevent sudden jumps
        rb.linearVelocity = Vector3.zero;
    }

    void OnMouseUp()
    {
        // Stop dragging
        isDragging = false;

        // Stop the object's movement when released
        rb.linearVelocity = Vector3.zero;
    }

    private Vector3 GetMouseWorldPosition()
    {
        // Get the mouse position in screen space and convert it to world space
        Vector3 screenPosition = Input.mousePosition;
        screenPosition.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        return mainCamera.ScreenToWorldPoint(screenPosition);
    }
}
