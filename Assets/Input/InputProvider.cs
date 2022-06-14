using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputProvider
{
    private PlayerInput input = new PlayerInput(); 

    public void Enable()
    {
        input.CameraMovement.Move.Enable();
        input.CameraMovement.MousePosition.Enable();
        input.GameplayActions.Enable();
    }

    public void Disable()
    {
        input.CameraMovement.Move.Disable();
        input.CameraMovement.MousePosition.Disable();
        input.CameraMovement.Zoom.Disable();
        input.GameplayActions.Disable();
    }

    public Vector2 CameraMovement()
    {
        return input.CameraMovement.Move.ReadValue<Vector2>();
    }

    public Vector2 MousePosition()
    {
        return input.CameraMovement.MousePosition.ReadValue<Vector2>();
    }

    public float MouseScrollWheel()
    {
        return input.CameraMovement.Zoom.ReadValue<float>();
    }

    public bool MouseLeftButton()
    {
        return input.GameplayActions.MouseLeftButton.phase.Equals(InputActionPhase.Performed);
    }
}
