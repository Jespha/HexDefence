using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Roads : MonoBehaviour
{

    // public static Roads Instance;
    private List<GameObject> _roads = new List<GameObject>();

    private void Start()
    {
    }
    // void Awake() => Instance = this;

    public void CreateRoad()
    {
        for (int i = 0; i <  HexGridManager.Instance.HexCells2DArray.GetLength(0); i++)
        {
            GameObject road = new GameObject("Road" + i);
            LineRenderer lineRenderer = road.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.widthMultiplier = 0.2f;
            lineRenderer.positionCount = 2;
            _roads.Add(road);
            Vector3 roadPos = this.transform.position +  new Vector3(0,1,0);
            Instantiate(road, this.transform);

            for (int j = 0; j < HexGridManager.Instance.HexCells2DArray.GetLength(j); j++)
            {
                lineRenderer.SetPosition(j, HexGridManager.Instance.HexCells[j].transform.position);
            }
        }
    }

    public void ClearRoads()
    {
        for (int i = 0; i < _roads.Count; i++)
        {
            UnityEngine.Object.DestroyImmediate(_roads[i]);
        }
    }

}
