using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInput : Singleton<PlayerInput>
{
    [Header("Links")]
    PlayerControls playerControls;
    public EventSystem eventSystem;

    [Header("Input")]
    public Vector2 _input;
    public float verticaLInput;
    public float horizontalInput;
    public float scrollInput;

    [Header("Mouse Events")]
    public Action OnLeftMouseClick;
    public Action OnRightMouseClick;
    public Action OnMiddleMouseClick;
    public Action <float> OnScrollWheel;
    public Action<Vector2> onCameraMove;
    

    private void OnEnable()
    {
        if(playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.CameraInGame.Move.performed += i => _input = i.ReadValue<Vector2>();
            playerControls.CameraInGame.Zoom.performed += i => scrollInput = i.ReadValue<float>();
            playerControls.Mouse.Left.performed += i => OnLeftMouseClick?.Invoke();
            playerControls.Mouse.Right.performed += i => OnRightMouseClick?.Invoke();

        }

        playerControls.Enable();

    }

    
    private void OnDisabled(){

        playerControls.Disable();

    }

    public void HandleAllInputs()
    {
        HandelMouseInput();
        HandleKeyboardInput();
    }

    private void HandelMouseInput()
    {
        if (eventSystem.IsPointerOverGameObject())
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnLeftMouseClick?.Invoke();
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            OnRightMouseClick?.Invoke();
        }

        if (Mouse.current.middleButton.wasPressedThisFrame)
        {
            OnMiddleMouseClick?.Invoke();
        }
    }

    private void HandleKeyboardInput()
    {
        verticaLInput = _input.y;
        horizontalInput = _input.x;
        if (_input != Vector2.zero)
        onCameraMove?.Invoke(_input);
        if (scrollInput != 0)
        OnScrollWheel?.Invoke(scrollInput);
    }

}
