using UnityEngine;

public class DragBaby : MonoBehaviour
{
    [Header("Drag Settings")]
    [SerializeField] private float dragSpeed = 10f; // Speed multiplier for dragging

    [SerializeField] private Camera _camera;
    private bool isDragging = false;
    private Vector3 targetPosition;

    void Update()
    {
        if (isDragging)
        {
            // Get the current position of the object
            Vector3 currentPosition = transform.position;

            // Update only the X position based on the mouse position
            Vector3 mousePosition = GetMouseWorldPosition();
            targetPosition = new Vector3(mousePosition.x, currentPosition.y, currentPosition.z);

            // Smoothly move the object toward the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * dragSpeed);
        }
    }

    void OnMouseDown()
    {
        // Start dragging
        isDragging = true;
        Debug.Log("Started Dragging");
    }

    void OnMouseUp()
    {
        // Stop dragging
        isDragging = false;
        GetComponent<MoveToObject>().MoveTo(0);
        Debug.Log("Stopped Dragging");
    }

    private Vector3 GetMouseWorldPosition()
    {
        // Get the mouse position in screen space
        Vector3 screenPosition = Input.mousePosition;

        // Calculate the distance from the camera to the object along the camera's forward direction
        screenPosition.z = Vector3.Dot(transform.position - _camera.transform.position, _camera.transform.forward);

        // Convert the screen position to world space
        return _camera.ScreenToWorldPoint(screenPosition);
    }
}
