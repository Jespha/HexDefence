using UnityEditor;

[CustomEditor(typeof(CameraManager))]
public class CameraManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        
        CameraManager _cameraManager = (CameraManager)target;
        DrawDefaultInspector();

        EditorGUILayout.LabelField("Min Val:", _cameraManager._minZoomRange.ToString());
        EditorGUILayout.LabelField("Max Val:", _cameraManager._maxZoomRange.ToString());
        EditorGUILayout.MinMaxSlider(
            ref _cameraManager._minZoomRange,
            ref _cameraManager._maxZoomRange,
            _cameraManager._minLimitZoomRange,
            _cameraManager._maxLimitZoomRange
        );  

    }
}
