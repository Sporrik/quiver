using UnityEngine;

public class DragBaby : MonoBehaviour
{
    [Header("Drag Settings")]
    public float dragSpeed = 10f; // Speed multiplier for dragging
    public float damping = 5f; // Damping to smooth the movement

    [Header("Position Limits")]
    public float minX = -5f; // Minimum X position
    public float maxX = 5f; // Maximum X position

    private Camera mainCamera;
    private bool isDragging = false;
    private Vector3 targetPosition;

    void Start()
    {
        // Cache the main camera reference
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (isDragging)
        {
            // Update the target position based on the mouse position
            Vector3 mousePosition = GetMouseWorldPosition();

            // Clamp the X position within the specified limits
            targetPosition.x = Mathf.Clamp(mousePosition.x, minX, maxX);

            // Smoothly move the object toward the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * damping);
        }
    }

    void OnMouseDown()
    {
        // Start dragging and set the initial target position
        isDragging = true;
        targetPosition = transform.position;
    }

    void OnMouseUp()
    {
        // Stop dragging
        isDragging = false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        // Get the mouse position in screen space and convert it to world space
        Vector3 screenPosition = Input.mousePosition;
        screenPosition.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        return mainCamera.ScreenToWorldPoint(screenPosition);
    }
}
