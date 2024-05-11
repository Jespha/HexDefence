using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using Sirenix.OdinInspector;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public static RoadManager Instance;

    [SerializeField]
    private Material _roadMaterial;

    [SerializeField]
    private PooledObject _portalPrefab;
    private List<PooledObject> _portals = new();

    [SerializeField]
    private GameObject _roadPrefab;
    RoadParent[] Roads = new RoadParent[6];

    [SerializeField]
    AnimationCurve _roadIntroCurve;

    private void Start() { }

    void OnEnable()
    {
        GameManager.Instance.OnLevelStart += OnLevelStart;
        GameManager.Instance.OnLevelComplete += OnLevelComplete;
    }

    void OnDisable()
    {
        GameManager.Instance.OnLevelStart -= OnLevelStart;
        GameManager.Instance.OnLevelComplete -= OnLevelComplete;
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
            new SplinePoint(StartPoint.transform.position + new Vector3(0, 0.35f, 0)),
            SplineComputer.Space.World
        );
        _splineComputer.SetPoint(
            1,
            new SplinePoint(EndPoint.transform.position + new Vector3(0, 0.35f, 0)),
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
                new SplinePoint(end.Position + new Vector3(0, 0.35f, 0)),
                SplineComputer.Space.World
            );
        Roads[roadIndex].splineMesh.GetChannel(0).count = pointCount + 1;
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

    private void OnLevelStart(int level, Level _level)
    {
        for (int i = 0; i < Roads.Length; i++)
        {
            if (Roads[i].gameObject != null)
            {
                PooledObject portal = PooledObjectManager.Instance.Get(_portalPrefab);
                portal.transform.position = Roads[i]
                    .splineComputer.GetPointPosition(
                        Roads[i].splineComputer.pointCount - 1,
                        SplineComputer.Space.World
                    );
                _portals.Add(portal);
            }
        }
    }

    private void OnLevelComplete(int level, Level _level)
    {
        if (_portals.Count > 0)
        {
            foreach (PooledObject portal in _portals)
            {
                PooledObjectManager.Instance.ReturnToPool(portal);
            }
        }
    }

    [Button("Animate Roads")]
    public void AnimateRoads()
    {
        for (int i = 0; i < Roads.Length; i++)
        {
            if (Roads[i].gameObject != null)
            {
                StartCoroutine(AnimateRoad(Roads[i]));
            }
        }
    }

    private IEnumerator AnimateRoad(RoadParent road)
    {
        float t = 0;
        Vector3 start = road.splineComputer.GetPointPosition(0, SplineComputer.Space.World);
        Vector3 end = road.splineComputer.GetPointPosition(1, SplineComputer.Space.World);
        road.splineComputer.SetPoint(1, new SplinePoint(end), SplineComputer.Space.World);
        MeshRenderer _meshRender = road.gameObject.GetComponent<MeshRenderer>();
        _meshRender.enabled = true;
        while (t < 1)
        {
            t += Time.deltaTime;
            float curveValue = _roadIntroCurve.Evaluate(t);
            Vector3 position = Vector3.Lerp(start, end, curveValue);
            road.splineComputer.SetPoint(1, new SplinePoint(position), SplineComputer.Space.World);
            road.splineMesh.Rebuild();
            yield return null;
        }
    }

    public void HideRoads()
    {
        for (int i = 0; i < Roads.Length; i++)
        {
            if (Roads[i].gameObject != null)
            {
                MeshRenderer _meshRender = Roads[i].gameObject.GetComponent<MeshRenderer>();
                _meshRender.enabled = false;
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
