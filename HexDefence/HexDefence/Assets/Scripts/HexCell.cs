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
    public List<Vector3> Neighbors  { get; private set; }
    GameObject buildingPrefab;

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

    private void BuildHexBuilding()
    {
        buildingPrefab = Instantiate(HexBuilding.Prefab.gameObject,this.transform);
        buildingPrefab.transform.position = this.transform.position;
    }

    public void RandomizeHeight()
    {
        Height = (Depth + 2 ) + UnityEngine.Random.Range(0f, 2.0f);
        gameObject.transform.localScale = new Vector3(1, Height, 1);
        if (buildingPrefab != null)
        {
            ScaleParentObjectButNotChild(gameObject, buildingPrefab, new Vector3(1, 1, 1));
            Debug.Log("BuildingPrefab is not null");
        }
    }

    public void Selected(RaycastHit hit)
    {
        HexGridManager.SelectHexCell(this);
        gameObject.GetComponent<Renderer>().material.color = Color.red;
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