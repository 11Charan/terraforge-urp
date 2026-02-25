using UnityEngine;
using UnityEngine.InputSystem;

public class FreeFlyCamera : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float mouseSensitivity = 2f;

    float rotationX = 0f;
    float rotationY = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        var mouse = Mouse.current;
        var keyboard = Keyboard.current;

        if (mouse == null || keyboard == null)
            return;

        // Mouse Look
        Vector2 lookDelta = mouse.delta.ReadValue();
        rotationX += lookDelta.x * mouseSensitivity;
        rotationY -= lookDelta.y * mouseSensitivity;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f);

        transform.rotation = Quaternion.Euler(rotationY, rotationX, 0f);

        // Movement
        float moveX = 0f;
        float moveZ = 0f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) moveX -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) moveX += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) moveZ -= 1f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) moveZ += 1f;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        if (keyboard.eKey.isPressed)
            move += Vector3.up;

        if (keyboard.qKey.isPressed)
            move += Vector3.down;

        transform.position += move * moveSpeed * Time.deltaTime;
    }
}