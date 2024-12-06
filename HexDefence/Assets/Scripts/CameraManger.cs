using System.Collections;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private CinemachineBrain _cinemachineBrain;

    [SerializeField]
    private PlayerInput _inputManager;
    [SerializeField] private CinemachineVirtualCamera _activeVirtualCamera;
    [SerializeField] private GameObject _preGameVCam;
    [SerializeField] private GameObject _menuVCam;
    public CameraState CurrentCameraState;

    private CinemachineTransposer _transposer;
    public float _minZoomRange;
    public float _maxZoomRange;
    public float _minLimitZoomRange = 14.0f;
    public float _maxLimitZoomRange = 32.0f;

    private float elapsedTime = 0f;
    private const float duration = 0.2f;
    private float targetOffset = 0f;

    public void StartInGameCamera()
    {
        _inputManager.onCameraMove += MoveCamera;
        _inputManager.OnScrollWheel += ZoomCamera;
    }

    public void EndInGameCamera()
    {
        _inputManager.onCameraMove -= MoveCamera;
        _inputManager.OnScrollWheel -= ZoomCamera;
    }

    private void Start()
    {
        _preGameVCam.SetActive(false);
    }

    private void OnDisable()
    {
        _inputManager.onCameraMove -= MoveCamera;
        _inputManager.OnScrollWheel -= ZoomCamera;
    }

    private void MoveCamera(Vector2 position)
    {
        if (_activeVirtualCamera != null)
        {
            var targetPos = _activeVirtualCamera.LookAt.position;
            targetPos.x += position.x * 0.5f;
            targetPos.z += position.y * 0.5f;
            _activeVirtualCamera.LookAt.position = targetPos;
        }
    }

    private void ZoomCamera(float level)
    {
        if (_activeVirtualCamera != null)
        {
            level = Mathf.Clamp(level, 1, -1);
            var targetPos = _activeVirtualCamera.Follow.position;
            if (targetPos.y + level > _maxZoomRange || targetPos.y + level < _minZoomRange)
                return;
            targetPos.y += level;
            targetPos.z = - targetPos.y; // Move the target in the Z direction
            _activeVirtualCamera.Follow.position = targetPos;
        }
    }

    public void SetActiveVirtualCamera(CameraState cameraState)
    {
        CurrentCameraState = cameraState;
        
        switch (cameraState)
        {
            case CameraState.PreGame:
                _preGameVCam.SetActive(true);
                break;
            case CameraState.Menu:
                _preGameVCam.SetActive(false);
             _menuVCam.SetActive(true);
                break;
            case CameraState.InGame:
                _preGameVCam.SetActive(false);
             _menuVCam.SetActive(false);
                break;
        }
    }

    public enum CameraState
    {
        PreGame,
        Menu,
        InGame
    }

}
