using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RotateObject : MonoBehaviour
{
    [Header("Axis Lock")]
    [SerializeField] private bool enableX = false;
    [SerializeField] private bool enableY = true;
    [SerializeField] private bool enableZ = false;

    [Header("Rotation Settings")]
    [SerializeField] private float torqueForce = 5f; // Force multiplier for torque
    [SerializeField] private float angularDamp = 2f; // Damping to smooth the rotation

    private Camera mainCamera;
    private Rigidbody rb;
    private bool isRotating = false;
    private Vector3 lastMousePosition;

    void Start()
    {
        // Cache the main camera and Rigidbody reference
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();

        // Ensure the Rigidbody is set up for physics-based rotation
        rb.useGravity = false; // Disable gravity if not needed
        rb.angularDamping = angularDamp; // Set angular drag for smoother rotation
    }

    void Update()
    {
        if (isRotating)
        {
            ApplyTorque();
        }
    }

    void OnMouseDown()
    {
        // Start rotating and cache the initial mouse position
        isRotating = true;
        lastMousePosition = Input.mousePosition;
    }

    void OnMouseUp()
    {
        // Stop rotating
        isRotating = false;
    }

    private void ApplyTorque()
    {
        // Calculate the mouse delta (movement)
        Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

        // Determine the torque to apply based on the enabled axes
        Vector3 torque = Vector3.zero;
        if (enableX)
        {
            torque.x = -mouseDelta.y * torqueForce; // Invert Y for intuitive rotation
        }
        if (enableY)
        {
            torque.y = mouseDelta.x * torqueForce;
        }
        if (enableZ)
        {
            torque.z = -mouseDelta.x * torqueForce; // Invert X for intuitive rotation
        }

        // Apply the torque to the Rigidbody
        rb.AddTorque(torque, ForceMode.Force);

        // Update the last mouse position
        lastMousePosition = Input.mousePosition;
    }
}
