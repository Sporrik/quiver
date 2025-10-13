using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public class CameraMovement : MonoBehaviour
{
    private Vector3 CameraPosition;


    [Header("CameraSettings")]
    public float CameraSpeed;

    private void Start()
    {
        CameraPosition = this.transform.position;
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.E))
        {
            CameraPosition.y += CameraSpeed / 10;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            CameraPosition.y -= CameraSpeed / 10;
        }
        if (Input.GetKey(KeyCode.A))
        {
            CameraPosition.x -= CameraSpeed / 10;
        }
        if (Input.GetKey(KeyCode.D))
        {
            CameraPosition.x += CameraSpeed / 10;
        }
        if (Input.GetKey(KeyCode.W))
        {
            CameraPosition.z += CameraSpeed / 10;
        }
        if (Input.GetKey(KeyCode.S))
        {
            CameraPosition.z -= CameraSpeed / 10;
        }

        if (Input.GetMouseButton(2))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

            this.transform.position = CameraPosition;
    }
}
