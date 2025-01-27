using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexGridManager HexGridManager { get; private set; }
    public Vector3 Position { get; private set; }
    public int Depth { get; private set; }
    public float Height { get; private set; }
    public HexTerrain HexTerrain { get; private set; }
    public HexBuilding HexBuilding { get; private set; }
    public List<Vector3> Neighbors;
    public bool IsTemp { get; private set; }
    private bool _buildingIsTemp = false;

    public int RoadIndex { get; private set; }
    public HexCell RoadEntryPoint { get; private set; }
    public HexCell RoadEndPoint { get; private set; }

    [SerializeField]
    private AnimationCurve _clickCurve;

    [SerializeField]
    private AnimationCurve _introCurve;

    [SerializeField]
    private float _duration = 1f;
    private PooledObject buildingPrefab;

    [SerializeField]
    private MeshRenderer _meshRenderer;

    public void Initialize(
        Vector3 position,
        int depth,
        float height,
        HexTerrain terrain,
        HexBuilding building,
        List<Vector3> neighbors,
        HexGridManager hexGridManager
    )
    {
        Position = position;
        Depth = depth;
        Height = height;
        HexTerrain = terrain;
        HexBuilding = building;
        Neighbors = neighbors;
        HexGridManager = hexGridManager;

        if (HexBuilding.HexBuildingType != HexBuildingType.None)
        {
            BuildHexBuilding();
        }
    }

    public void InitializationTemp(bool isTemp, float offset)
    {
        IsTemp = isTemp;
        StartCoroutine(AnimateScaleHeightCoroutine(this.transform, offset));
        this.gameObject.layer = 10;

        _meshRenderer.renderingLayerMask = _meshRenderer.renderingLayerMask + 2;
        _meshRenderer.material.SetFloat("_selected", 1);
        _meshRenderer.material.SetFloat("_Temp", 1);
    }

    public void SetTempBuilding()
    {
        _buildingIsTemp = true;
        buildingPrefab = PooledObjectManager.Instance.Get(GameManager.Instance.TempBuilding.Prefab);
        buildingPrefab.transform.position = this.transform.position;
        // StartCoroutine(AnimateScaleCoroutine(this.transform));
    }

    public void RevertTempBuilding()
    {
        PooledObjectManager.Instance.ReturnToPool(buildingPrefab);
        if (_buildingIsTemp)
        {
            buildingPrefab = null;
            HexBuilding = Resources.Load<HexBuilding>("ScriptableObjects/HexBuilding/None");
        }
    }

    public void SetNeighbors(List<Vector3> neighbors)
    {
        Neighbors = neighbors;
    }

    public void SetRoad(HexCell hexcell, RoadType roadType)
    {
        if (roadType == RoadType.Entry)
        {
            RoadEntryPoint = hexcell;
        }
        else
        {
            RoadEndPoint = hexcell;
        }
    }

    public void SetRoad(HexCell hexcell, RoadType roadType, int roadIndex)
    {
        RoadIndex = roadIndex;
        if (roadType == RoadType.Entry)
        {
            RoadEntryPoint = hexcell;
        }
        else
        {
            RoadEndPoint = hexcell;
        }
    }

    private void BuildHexBuilding()
    {
        buildingPrefab = Instantiate(HexBuilding.Prefab, this.transform);
        buildingPrefab.transform.position = this.transform.position;
        buildingPrefab.GetComponent<MeshRenderer>().renderingLayerMask = 1;
    }

    public void BuildHexBuilding(HexBuilding hexBuilding)
    {
        _buildingIsTemp = false;
        RevertTempBuilding();
        HexBuilding = GameManager.Instance.TempBuilding;
        buildingPrefab = Instantiate(HexBuilding.Prefab, this.transform);
        buildingPrefab.transform.position = this.transform.position;
        StartCoroutine(AnimateScaleCoroutine(this.transform));
        HexGridManager.AddTower(this, buildingPrefab);
    }

    public void RandomizeHeight()
    {
        Height = (Depth + 2) + UnityEngine.Random.Range(0f, 2.0f);
        gameObject.transform.localScale = new Vector3(1, Height, 1);
        if (buildingPrefab != null)
        {
            ScaleParentObjectButNotChild(
                gameObject,
                buildingPrefab.gameObject,
                new Vector3(1, 1, 1)
            );
        }
    }

    private void SetHeight()
    {
        gameObject.transform.localScale = new Vector3(1, Height, 1);
        if (buildingPrefab != null)
        {
            ScaleParentObjectButNotChild(
                gameObject,
                buildingPrefab.gameObject,
                new Vector3(1, 1, 1)
            );
        }
    }

    public void Selected()
    {
        StartCoroutine(AnimateScaleCoroutine(this.transform));
        _meshRenderer.material.SetFloat("_Selected", 1);

        if (!IsTemp)
        {
            this.gameObject.layer = 12;
            if (Currency.Instance.HexCurrency >= 1)
                ShowNeighbors();
        }
    }

    public void Deselected()
    {
        if (IsTemp)
        {
            _meshRenderer.material.SetFloat("_Selected", 0);
            return;
        }
        else
        {
            _meshRenderer.material = null;
            _meshRenderer.sharedMaterial = HexTerrain.Material;
            this.gameObject.layer = 6;
        }
    }

    private void ShowNeighbors()
    {
        foreach (var neighbor in Neighbors)
        {
            if (!HexGridManager.PositionExistsInList(HexGridManager.HexCells, neighbor))
            {
                HexGridManager.InstantiateTempHexagon(neighbor, Depth + 1);
            }
        }
    }

    private IEnumerator AnimateScaleCoroutine(Transform _transform)
    {
        float time = 0;

        while (time < _duration)
        {
            float scale = _clickCurve.Evaluate(time / _duration);
            _transform.localScale = new Vector3(scale, scale, scale);

            time += Time.deltaTime;
            yield return null;
        }

        float finalScale = _clickCurve.Evaluate(1);
        _transform.localScale = new Vector3(finalScale, finalScale, finalScale);
    }

    private IEnumerator AnimateScaleHeightCoroutine(Transform _transform, float offset)
    {
        float time = 0;
        float y = _transform.localPosition.y;
        if (offset != 0)
            // _duration = _duration + (offset*0.175f);
            this.GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(offset * 0.075f);
        this.GetComponent<MeshRenderer>().enabled = true;
        while (time < _duration)
        {
            float scale = y + _clickCurve.Evaluate(time / _duration);
            _transform.localPosition = new Vector3(
                _transform.localPosition.x,
                scale,
                _transform.localPosition.z
            );

            time += Time.deltaTime;
            yield return null;
        }
    }

    private void ScaleParentObjectButNotChild(
        GameObject parentObject,
        GameObject childObject,
        Vector3 initialChildScale
    )
    {
        if (!IsValidParentChild(parentObject, childObject))
            return;
        Vector3 parentScale = parentObject.transform.localScale;
        Vector3 newChildScale = new Vector3(
            initialChildScale.x / parentScale.x,
            initialChildScale.y / parentScale.y,
            initialChildScale.z / parentScale.z
        );
        childObject.transform.localScale = newChildScale;
        childObject.transform.localPosition = new Vector3(0, -1 + Mathf.Pow(Height, 0.12f));
    }

    private bool IsValidParentChild(GameObject parentObject, GameObject childObject)
    {
        if (childObject.transform.parent == parentObject.transform)
            return true;
        Debug.LogError("Objects are not in Parent-Child relationship");
        return false;
    }

    public void DestroyBuilding()
    {
        if (buildingPrefab != null)
        {
            Destroy(buildingPrefab.gameObject);
            HexBuilding = null;
        }
    }

    public void AnimateIn(float _wait = 0.0f)
    {
        Transform _endPos = this.transform;
        Vector3 _startPos = new Vector3(0, -6, 0);
        _meshRenderer.enabled = false;
        StartCoroutine(
            AnimationCoroutine.AnimatePositionCoroutine(
                _endPos,
                _startPos,
                _wait * 0.025f,
                0.55f,
                _introCurve,
                _meshRenderer
            )
        );
    }

    public void AnimateFallIn()
    {
        Transform _endPos = this.transform;
        Vector3 _startPos = new Vector3(0, 15, 0);
        _meshRenderer.enabled = true;
        buildingPrefab.gameObject.SetActive(true);
        StartCoroutine(
            AnimationCoroutine.AnimatePositionCoroutine(_endPos, _startPos, 0, 0.6f, _introCurve)
        );
    }

    public void HideHexCell()
    {
        _meshRenderer.enabled = false;
        if (buildingPrefab != null)
            buildingPrefab.gameObject.SetActive(false);
    }
}

[Serializable]
public enum RoadType
{
    Entry,
    Exit,
    LastPoint,
    None,
}
