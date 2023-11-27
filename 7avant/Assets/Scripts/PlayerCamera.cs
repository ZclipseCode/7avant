using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Transform orientation;
    [SerializeField] float xSensitivity = 0.1f;
    [SerializeField] float ySensitivity = 0.1f;
    float xRotation;
    float yRotation;
    PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Enable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = playerControls.Player.LookX.ReadValue<float>() * xSensitivity;
        float mouseY = playerControls.Player.LookY.ReadValue<float>() * ySensitivity;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void OnDestroy()
    {
        playerControls.Player.Disable();
    }
}
