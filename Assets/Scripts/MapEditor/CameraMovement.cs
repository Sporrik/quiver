#if UNITY_EDITOR
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float cameraSpeed = 1f;
    [SerializeField] private float cameraRotationSpeed = 100f;

    private Vector3 _cameraPosition;
    private float _cameraRotation;
    private float _baseCameraSpeed;
    private void Start()
    {
        _cameraPosition = transform.position;
        _cameraRotation = transform.eulerAngles.y;
        cameraSpeed /= 10;
        cameraRotationSpeed /= 10;
        _baseCameraSpeed = cameraSpeed;
    }

    private void FixedUpdate()
    {
        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 right = new Vector3(transform.right.x, 0, transform.right.z).normalized;

        // Movement input
        if (Input.GetKey(KeyCode.LeftShift))
        {
            cameraSpeed = _baseCameraSpeed * 3;
        }
        else
        {
            cameraSpeed = _baseCameraSpeed;
        }
        if (Input.GetKey(KeyCode.W))
        {
            _cameraPosition += forward * cameraSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            _cameraPosition -= forward * cameraSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            _cameraPosition += right * cameraSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            _cameraPosition -= right * cameraSpeed;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            _cameraPosition.y -= cameraSpeed;
        }
        if (Input.GetKey(KeyCode.E))
        {
            _cameraPosition.y += cameraSpeed;
        }

        if (Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            _cameraRotation += mouseX * cameraRotationSpeed;
        }

        transform.position = _cameraPosition;
        transform.rotation = Quaternion.Euler(60f, _cameraRotation, 0); // Keep X locked at 60°
    }
}
#endif