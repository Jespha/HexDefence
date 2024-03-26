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
    public Action <Vector2> onCameraMove;
    public Action <bool> MultiBuildMode;
    public Action <bool> UpgradeBuildMode;
    public Action <int> BuildMode;


    private void OnEnable()
    {
        if(playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.CameraInGame.Move.performed += i => _input = i.ReadValue<Vector2>();
            playerControls.CameraInGame.Zoom.performed += i => scrollInput = i.ReadValue<float>();
            playerControls.Mouse.Left.performed += i => OnLeftMouseClick?.Invoke();
            playerControls.Mouse.Right.performed += i => OnRightMouseClick?.Invoke();
            playerControls.BuildKeys.MultiBuild.performed += i => MultiBuildMode?.Invoke(true);
            playerControls.BuildKeys.MultiBuild.canceled += i => MultiBuildMode?.Invoke(false);
            playerControls.BuildKeys.Upgrade.performed += i => UpgradeBuildMode?.Invoke(true);
            playerControls.BuildKeys.Upgrade.canceled += i => UpgradeBuildMode?.Invoke(false);
            playerControls.BuildKeys.One.performed += i => BuildMode?.Invoke(1);
            playerControls.BuildKeys.Two.performed += i => BuildMode?.Invoke(2);
            playerControls.BuildKeys.Three.performed += i => BuildMode?.Invoke(3);
            playerControls.BuildKeys.Four.performed += i => BuildMode?.Invoke(4);
            playerControls.BuildKeys.Five.performed += i => BuildMode?.Invoke(5);
            playerControls.BuildKeys.Six.performed += i => BuildMode?.Invoke(6);
            playerControls.BuildKeys.Seven.performed += i => BuildMode?.Invoke(7);
            playerControls.BuildKeys.Eight.performed += i => BuildMode?.Invoke(8);
            playerControls.BuildKeys.Nine.performed += i => BuildMode?.Invoke(9);
            playerControls.BuildKeys.Zero.performed += i => BuildMode?.Invoke(0);
        }

        playerControls.Enable();

    }

    
    private void OnDisabled(){

        playerControls.Disable();

    }

    public void HandleAllInputs()
    {
        HandleKeyboardInput();
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
