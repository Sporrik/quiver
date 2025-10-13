using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DragObject : MonoBehaviour
{
    [Header("Axis Lock")]
    public bool enableX = false; // Enable dragging on the X-axis
    public bool enableY = false; // Enable dragging on the Y-axis
    public bool enableZ = false; // Enable dragging on the Z-axis

    [Header("Position Limits")]
    public Vector2 xLimits = new Vector2(float.MinValue, float.MaxValue); // Min and Max limits for X-axis
    public Vector2 yLimits = new Vector2(float.MinValue, float.MaxValue); // Min and Max limits for Y-axis
    public Vector2 zLimits = new Vector2(float.MinValue, float.MaxValue); // Min and Max limits for Z-axis

    [Header("Physics Settings")]
    public float dragSpeed = 10f; // Speed multiplier for dragging
    public float damping = 5f; // Damping to smooth the movement
    public bool useGravityOnRelease = true; // Whether gravity should be applied when the object is released
    public bool StopCopyRotation = false; // Whether to stop copying rotation
    public bool ModifyLinearDamping = false; // Whether to modify linear drag
    public float LinearDampingValue = 0f; // Value to set for linear drag if ModifyLinearDamping is true
    public bool UnlockBones = false; // Whether to unlock bones on release

    private Camera mainCamera;
    private Rigidbody rb;
    private bool isDragging = false;
    private Vector3 targetPosition;

    private Vector2 relativeXLimits;
    private Vector2 relativeYLimits;
    private Vector2 relativeZLimits;
    private float _startingLinearDamping;

    void Start()
    {
        // Cache the main camera and Rigidbody reference
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = false; // Rigidbody must not be kinematic for physics interactions

        // Calculate relative limits based on the object's starting position
        relativeXLimits = new Vector2(transform.position.x + xLimits.x, transform.position.x + xLimits.y);
        relativeYLimits = new Vector2(transform.position.y + yLimits.x, transform.position.y + yLimits.y);
        relativeZLimits = new Vector2(transform.position.z + zLimits.x, transform.position.z + zLimits.y);
    }

    void Update()
    {
        if (isDragging)
        {
            // Update the target position based on the mouse position
            Vector3 mousePosition = GetMouseWorldPosition();

            // Apply axis locking and relative limits
            if (enableX)
            {
                targetPosition.x = Mathf.Clamp(mousePosition.x, relativeXLimits.x, relativeXLimits.y);
            }
            if (enableY)
            {
                targetPosition.y = Mathf.Clamp(mousePosition.y, relativeYLimits.x, relativeYLimits.y);
            }
            if (enableZ)
            {
                targetPosition.z = Mathf.Clamp(mousePosition.z, relativeZLimits.x, relativeZLimits.y);
            }
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

        // Disable gravity while dragging
        rb.useGravity = false;

        // Stop any existing velocity to prevent sudden jumps
        rb.linearVelocity = Vector3.zero;

        // Optionally modify linear drag
        if (ModifyLinearDamping)
        {
            _startingLinearDamping = rb.linearDamping;
            rb.linearDamping = LinearDampingValue;
        }

        // Unlock bones
        if (UnlockBones)
        {
            UnlockDrag dragRef = gameObject.GetComponent<UnlockDrag>();
            dragRef.FrontLock.UnlockBones();
            dragRef.BackLock.UnlockBones();
        }
    }

    void OnMouseUp()
    {
        // Stop dragging
        isDragging = false;

        // Apply gravity based on the useGravityOnRelease flag
        rb.useGravity = useGravityOnRelease;

        // Stop the object's movement when released
        if (!rb.isKinematic)
            rb.linearVelocity = Vector3.zero;

        // Optionally stop copying rotation
        if (StopCopyRotation)
        {
            gameObject.GetComponent<CopyRotation>().enabled = false;
        }

        // Reset linear drag to default
        if (ModifyLinearDamping)
        {
            rb.linearDamping = _startingLinearDamping;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        // Get the mouse position in screen space and convert it to world space
        Vector3 screenPosition = Input.mousePosition;
        screenPosition.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        return mainCamera.ScreenToWorldPoint(screenPosition);
    }
}