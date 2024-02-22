using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class Roads : MonoBehaviour
{

    // public static Roads Instance;
    [SerializeField] private Material _roadMaterial;
    [SerializeField] private GameObject _roadPrefab;
    [SerializeField] public List<GameObject> _roads = new List<GameObject>();

    private void Start()
    {
    }
    // void Awake() => Instance = this;

    public void CreateRoad(HexCell StartPoint, HexCell EndPoint)
    {
        GameObject road = Instantiate(_roadPrefab, this.transform);
        road.TryGetComponent(out SplineComputer roadSpline);
        road.TryGetComponent(out SplineMesh roadSplineMesh);
        roadSpline.SetPoint(0, new SplinePoint(StartPoint.transform.position), SplineComputer.Space.World);
        roadSpline.SetPoint(1, new SplinePoint(EndPoint.transform.position), SplineComputer.Space.World);
        roadSplineMesh.Rebuild();
        _roads.Add(road);
    }

    public void ClearRoads()
    {
        for (int i = _roads.Count - 1; i >= 0; --i)
        {
            DestroyImmediate(_roads[i].gameObject,true);
        }
        _roads.Clear();
    }

}
