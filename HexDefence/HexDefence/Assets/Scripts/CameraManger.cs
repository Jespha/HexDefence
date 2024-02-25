using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManger : MonoBehaviour
{
    CinemachineBrain _cinemachineBrain;

    private void Start()
    {
        if (_cinemachineBrain == null)
            _cinemachineBrain = FindObjectOfType<CinemachineBrain>();
        else
            Debug.Log("CinemachineBrain not found in CameraManager");
    }

    public void MoveCamera(Vector3 position)
    {
        
    }

    public void AimCamera(Vector3 position)
    {
        
    }

}
