using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexGridManager HexGridManager { get; private set; }
    public Vector3 Position { get; private set; }
    public int Depth { get; private set; }
    public float Height { get; private set; }
    public HexTerrain HexTerrain  { get; private set; }
    public HexBuilding HexBuilding  { get; private set; }
    public List<Vector3> Neighbors ;

    [SerializeField]private AnimationCurve _clickCurve;
    [SerializeField]private float _duration = 1f;
    private Building buildingPrefab;

    public void Initialize(Vector3 position, int depth, float height, HexTerrain terrain, HexBuilding building, List<Vector3> neighbors, HexGridManager hexGridManager)
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

    public void SetNeighbors(List<Vector3> neighbors)
    {
        Neighbors = neighbors;
    }

    private void BuildHexBuilding()
    {
        buildingPrefab = Instantiate(HexBuilding.Prefab,this.transform);
        buildingPrefab.Initialize(this);
        buildingPrefab.transform.position = this.transform.position;
    }

    public void BuildHexBuilding(HexBuilding hexBuilding)
    {
        HexBuilding = hexBuilding;
        buildingPrefab = Instantiate(HexBuilding.Prefab,this.transform);
        buildingPrefab.Initialize(this);
        buildingPrefab.transform.position = this.transform.position;
        StartCoroutine(AnimateScaleCoroutine(this.transform));
    }

    public void RandomizeHeight()
    {
        Height = (Depth + 2 ) + UnityEngine.Random.Range(0f, 2.0f);
        gameObject.transform.localScale = new Vector3(1, Height, 1);
        if (buildingPrefab != null)
        {
            ScaleParentObjectButNotChild(gameObject, buildingPrefab.gameObject, new Vector3(1, 1, 1));
            Debug.Log("BuildingPrefab is not null");
        }
    }

    public void Selected()
    {
        StartCoroutine(AnimateScaleCoroutine(this.transform));
        ShowNeighbors();
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

        // ensure the final scale is set correctly
        float finalScale = _clickCurve.Evaluate(1);
        _transform.localScale = new Vector3(finalScale, finalScale, finalScale);
    }

    private void ScaleParentObjectButNotChild(GameObject parentObject, GameObject childObject, Vector3 initialChildScale)
    {
        if (!IsValidParentChild(parentObject, childObject))
        return;
        Vector3 parentScale = parentObject.transform.localScale;
        Vector3 newChildScale = new Vector3(initialChildScale.x / parentScale.x, initialChildScale.y / parentScale.y, initialChildScale.z / parentScale.z);
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

}

    [Serializable]
    public enum HexTerrainType
    {
        Grass,
        GrassArrid,
        GrassLush,
        Desert,
        Mountain,
        Road,
    }

    [Serializable]
    public enum HexBuildingType
    {
        None,
        Base,
        BasicTower,
    }