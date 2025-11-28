using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 CameraPosition;
    private float CameraRotation;

    [Header("Camera Settings")]
    [SerializeField] private float CameraSpeed = 1f;
    [SerializeField] private float CameraRotationSpeed = 100f;

    private float BaseCameraSpeed;
    private void Start()
    {
        CameraPosition = transform.position;
        CameraRotation = transform.eulerAngles.y;
        CameraSpeed /= 10;
        CameraRotationSpeed /= 10;
        BaseCameraSpeed = CameraSpeed;
    }

    private void FixedUpdate()
    {
        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 right = new Vector3(transform.right.x, 0, transform.right.z).normalized;

        // Movement input
        if (Input.GetKey(KeyCode.LeftShift))
        {
            CameraSpeed = BaseCameraSpeed * 3;
        }
        else
        {
            CameraSpeed = BaseCameraSpeed;
        }
        if (Input.GetKey(KeyCode.W))
        {
            CameraPosition += forward * CameraSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            CameraPosition -= forward * CameraSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            CameraPosition += right * CameraSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            CameraPosition -= right * CameraSpeed;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            CameraPosition.y -= CameraSpeed;
        }
        if (Input.GetKey(KeyCode.E))
        {
            CameraPosition.y += CameraSpeed;
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
            CameraRotation += mouseX * CameraRotationSpeed;
        }

        transform.position = CameraPosition;
        transform.rotation = Quaternion.Euler(60f, CameraRotation, 0); // Keep X locked at 60°
    }
}
