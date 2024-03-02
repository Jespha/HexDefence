using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private CinemachineBrain _cinemachineBrain;

    [SerializeField]
    private PlayerInput _inputManager;
    private CinemachineVirtualCamera _activeVirtualCamera;
    private CinemachineTransposer _transposer;
    public float _minZoomRange;
    public float _maxZoomRange;
    public float _minLimitZoomRange = 14.0f;
    public float _maxLimitZoomRange = 32.0f;

    private float elapsedTime = 0f;
    private const float duration = 0.2f;
    private float targetOffset = 0f;

    private void OnEnable()
    {
        StartCoroutine(WaitForActiveCamera());
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

    private IEnumerator WaitForActiveCamera()
    {
        // Wait until the active camera is set
        while (_cinemachineBrain.ActiveVirtualCamera == null)
        {
            yield return null; // Wait for next frame
        }

        // Get the active camera from the CinemachineBrain
        ICinemachineCamera activeCamera = _cinemachineBrain.ActiveVirtualCamera;

        // Cast the active camera to CinemachineVirtualCamera
        _activeVirtualCamera = activeCamera as CinemachineVirtualCamera;

        // Check if the active camera was successfully cast
        if (_activeVirtualCamera == null)
        {
            Debug.LogError(
                "ActiveVirtualCamera could not be cast to CinemachineVirtualCamera in CameraManager"
            );
            yield break;
        }

        _inputManager.onCameraMove += MoveCamera;
        _inputManager.OnScrollWheel += ZoomCamera;
    }

}
