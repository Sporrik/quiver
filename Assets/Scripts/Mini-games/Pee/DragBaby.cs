using UnityEngine;
using UnityEngine.InputSystem;

public class DragBaby : MonoBehaviour
{
    [Header("Drag Settings")]
    [SerializeField] private float dragSpeed = 10f; // Speed multiplier for dragging
    [SerializeField] private GameObject _leftArrow;
    [SerializeField] private GameObject _rightArrow;

    [Header("Boundary Colliders")]
    [SerializeField] private Collider leftBoundary; // Left boundary collider
    [SerializeField] private Collider rightBoundary; // Right boundary collider

    private float horizontalInput = 0f; // Input value from the horizontal axis

    void Update()
    {
        // Calculate the new position based on horizontal input (negate to fix reversed controls)
        Vector3 newPosition = transform.position + new Vector3(-horizontalInput * dragSpeed * Time.deltaTime, 0, 0);

        // Check if the new position is within the boundaries
        if (leftBoundary != null && newPosition.x < leftBoundary.bounds.max.x)
        {
            newPosition.x = leftBoundary.bounds.max.x;
        }

        if (rightBoundary != null && newPosition.x > rightBoundary.bounds.min.x)
        {
            newPosition.x = rightBoundary.bounds.min.x;
        }

        // Apply the new position
        transform.position = newPosition;

        // Update arrow visibility based on input
        if (horizontalInput != 0f)
        {
            _leftArrow.SetActive(false);
            _rightArrow.SetActive(false);
        }
        else
        {
            _leftArrow.SetActive(true);
            _rightArrow.SetActive(true);

            // Call MoveTo(0) when input is 0
            // GetComponent<MoveToObject>().MoveTo(0);
        }
    }

    // Input System callback for horizontal movement
    public void OnMove(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<float>();
    }
}
