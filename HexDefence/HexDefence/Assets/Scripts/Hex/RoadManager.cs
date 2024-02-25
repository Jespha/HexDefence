using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class RoadManager : MonoBehaviour
{

    public static RoadManager Instance;
    [SerializeField] private Material _roadMaterial;
    [SerializeField] private GameObject _roadPrefab;
    [SerializeField] public List<GameObject> _roads = new List<GameObject>();
    [SerializeField] private SplineComputer _splineComputer;
    [SerializeField] private SplineMesh _splinMesh;

    private void Start()
    {
    }
    void Awake() => Instance = this;

    public void CreateRoad(HexCell StartPoint, HexCell EndPoint, int roadIndex)
    {
        
        GameObject road = Instantiate(_roadPrefab, this.transform);
        road.name = "Road" + roadIndex;
        road.TryGetComponent(out SplineComputer _splineComputer);
        road.TryGetComponent(out SplineMesh _splinMesh);

        _splineComputer.SetPoint(0, new SplinePoint(StartPoint.transform.position), SplineComputer.Space.World);
        _splineComputer.SetPoint(1, new SplinePoint(EndPoint.transform.position), SplineComputer.Space.World);
        _splinMesh.Rebuild();
        _roads.Add(road);
    }


    public void AddRoad(HexCell end, HexCell start)
    {
        // Debug.Log("RoadIndex: " + roadIndex);
        int roadIndex = start.RoadIndex;
        string roadName = "Road" + roadIndex.ToString();
        GameObject currentRoad = _roads.Find(x => x.name == roadName);


        // Debug.Log("Road with name " + currentRoad.name);


        currentRoad.TryGetComponent(out SplineComputer _splineComputer);
        currentRoad.TryGetComponent(out SplineMesh _splinMesh);
        int pointCount = _splineComputer.pointCount;

        _splineComputer.SetPoint( pointCount, new SplinePoint(end.Position), SplineComputer.Space.World);
        _splinMesh.Rebuild();
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
