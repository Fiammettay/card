using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager
{
    private PlayerInput inputActions;

    public Vector2 MoveInput {  get; private set; }

    public Vector2 MouseInput { get; private set; }

    public bool IsSprint {  get; private set; }

    public bool IsJump { get; private set; }

    public bool IsInteract {  get; private set; }

    public bool IsUnLock { get; private set; }

    public PlayerInputManager()
    {
        inputActions = new PlayerInput();

        inputActions.Player.Move.performed += ctx =>
        {
            MoveInput = ctx.ReadValue<Vector2>();
        };

        inputActions.Player.Move.canceled += ctx =>
        {
            MoveInput = Vector2.zero;
        };

        inputActions.Player.Mouse.performed += ctx =>
        {
            MouseInput = ctx.ReadValue<Vector2>();
        };

        inputActions.Player.Mouse.canceled += ctx =>
        {
            MouseInput = Vector2.zero;
        };

        inputActions.Player.Sprint.performed += ctx =>
        {
            IsSprint = true;
        };

        inputActions.Player.Sprint.canceled += ctx =>
        {
            IsSprint = false;
        };

        inputActions.Player.Jump.started += ctx =>
        {
            IsJump = true;
        };

        inputActions.Player.Jump.canceled += ctx =>
        {
            IsJump = false;
        };

        inputActions.Player.Interact.started += ctx =>
        {
            IsInteract = true;
        };

        inputActions.Player.Interact.canceled += ctx =>
        {
            IsInteract = false;
        };

        inputActions.Player.UnLock.performed += ctx =>
        {
            IsUnLock = true;
        };

        inputActions.Player.UnLock.canceled += ctx =>
        {
            IsUnLock = false;
        };

        inputActions.Enable();
    }

    public void Update(float dt)
    {
        IsJump = false;
        IsInteract = false;//true只保持一帧，防止一直按着
    }

    public void SetEnabled(bool enabled)
    {
        if(enabled)
        {
            inputActions.Enable();
        }
        else
        {
            inputActions.Disable();
        }
    }
}
