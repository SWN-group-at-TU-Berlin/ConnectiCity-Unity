using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] float movementSpeed = 20f;
    [SerializeField] float zoomSpeed = 100f;
    [SerializeField] float screenBorderPan = 0.5f;
    private InputProvider input;

    private void OnEnable()
    {
        input = new InputProvider();
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }
    private void Update()
    {
        KeysMovement();
        MouseMovement();

        if (input.MouseScrollWheel() > 0)
        {
            Vector3 direction = Camera.main.transform.forward;
            Vector3 zoom = direction * zoomSpeed * Time.deltaTime;
            transform.position += zoom;
        }

        if (input.MouseScrollWheel() < 0)
        {
            Vector3 direction = Camera.main.transform.forward;
            Vector3 zoom = direction * zoomSpeed * Time.deltaTime;
            transform.position -= zoom;
        }

    }

    private void MouseMovement()
    {
        //Mouse pos is calculated starting at (0,0) referencing the lower left corner of the screen

        if (input.MousePosition().y >= Screen.height - screenBorderPan)
        {
            float movementUp = -1 * movementSpeed * Time.deltaTime;
            transform.position += new Vector3(0f, 0f, movementUp);
        }

        if (input.MousePosition().y <= screenBorderPan)
        {
            float movementDown = 1 * movementSpeed * Time.deltaTime;
            transform.position += new Vector3(0f, 0f, movementDown);
        }

        if (input.MousePosition().x >= Screen.width - screenBorderPan)
        {
            float movementRight = -1 * movementSpeed * Time.deltaTime;
            transform.position += new Vector3(movementRight, 0f, 0f);
        }

        if (input.MousePosition().x <= screenBorderPan)
        {
            float movementLeft = 1 * movementSpeed * Time.deltaTime;
            transform.position += new Vector3(movementLeft, 0f, 0f);
        }
    }

    private void KeysMovement()
    {
        if (input.CameraMovement().magnitude > 0)
        {
            Vector2 movementOffset = -input.CameraMovement() * movementSpeed * Time.deltaTime;
            Vector3 movement = new Vector3(movementOffset.x, 0f, movementOffset.y);
            transform.position += movement;
        }
    }
}
