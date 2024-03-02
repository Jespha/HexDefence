using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public static RoadManager Instance;

    [SerializeField]
    private Material _roadMaterial;

    [SerializeField]
    private GameObject _portalPrefab;
    private List<GameObject> _portals = new();

    [SerializeField]
    private GameObject _roadPrefab;
    RoadParent[] Roads = new RoadParent[6];

    private void Start() { }

    void OnEnable()
    {
        GameManager.Instance.OnLevelStart += OnLevelStart;
        GameManager.Instance.OnLevelStart += OnLevelComplete;
    }

    void OnDisable()
    {
        GameManager.Instance.OnLevelStart -= OnLevelStart;
        GameManager.Instance.OnLevelStart -= OnLevelComplete;
    }

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one RoadManager in the scene!");
            return;
        }
        Instance = this;
    }

    public void CreateRoad(HexCell StartPoint, HexCell EndPoint, int roadIndex)
    {
        if (roadIndex < 0 || roadIndex >= Roads.Length)
        {
            Debug.LogError("Road index is out of bounds!");
            return;
        }
        GameObject road = Instantiate(_roadPrefab, this.transform);
        road.name = "Road" + roadIndex;
        road.TryGetComponent(out SplineComputer _splineComputer);
        road.TryGetComponent(out SplineMesh _splinMesh);
        _splineComputer.SetPoint(
            0,
            new SplinePoint(StartPoint.transform.position),
            SplineComputer.Space.World
        );
        _splineComputer.SetPoint(
            1,
            new SplinePoint(EndPoint.transform.position),
            SplineComputer.Space.World
        );
        _splinMesh.Rebuild();
        Roads[roadIndex] = new RoadParent
        {
            gameObject = road,
            splineComputer = _splineComputer,
            splineMesh = _splinMesh
        };
    }

    public void AddRoad(HexCell end, HexCell start)
    {
        int roadIndex = start.RoadIndex;
        if (roadIndex < 0 || roadIndex >= Roads.Length || Roads[roadIndex].gameObject == null)
        {
            Debug.LogError("Invalid road index or road does not exist!");
            return;
        }
        string roadName = "Road" + roadIndex.ToString();
        int pointCount = Roads[roadIndex].splineComputer.pointCount;
        Roads[roadIndex]
            .splineComputer.SetPoint(
                pointCount,
                new SplinePoint(end.Position),
                SplineComputer.Space.World
            );
        Roads[roadIndex].splineMesh.Rebuild();
    }

    public void ClearRoads()
    {
        if (Roads == null)
            return;

        for (int i = Roads.Length - 1; i >= 0; --i)
        {
            if (Roads[i].gameObject != null)
            {
                DestroyImmediate(Roads[i].gameObject, true);
            }
        }
        Roads = new RoadParent[6];
    }

    public RoadParent GetRandomRoad()
    {
        if (Roads.Length == 0)
            return default(RoadParent);

        RoadParent randomRoad = default(RoadParent);
        while (randomRoad.gameObject == null)
        {
            int randomIndex = Random.Range(0, Roads.Length);
            randomRoad = Roads[randomIndex];
        }
        return randomRoad;
    }

    private void OnLevelStart()
    {
        for (int i = 0; i < Roads.Length; i++)
        {
            if (Roads[i].gameObject != null)
            {
                GameObject portal = Instantiate(
                    _portalPrefab,
                    Roads[i]
                        .splineComputer.GetPointPosition(
                            Roads[i].splineComputer.pointCount - 1,
                            SplineComputer.Space.World
                        ),
                    Quaternion.identity
                );
                _portals.Add(portal);
            }
        }
    }

    private void OnLevelComplete()
    {
        if (_portals.Count > 0)
        {
            foreach (GameObject portal in _portals)
            {
                Destroy(portal);
            }
        }
    }
}

public struct RoadParent
{
    public GameObject gameObject;
    public SplineComputer splineComputer;
    public SplineMesh splineMesh;
}
