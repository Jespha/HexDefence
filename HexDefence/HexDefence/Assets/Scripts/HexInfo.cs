using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexInfo : MonoBehaviour
{
    [SerializeField] private HexTerrain _grass;
    [SerializeField] private HexBuilding _none;

    public Vector3 Position { get; set; }
    public int Depth { get; set; }
    public float Height { get; set; }
    private HexTerrain hexTerrain { get; set; }
    private HexBuilding hexBuilding { get; set; }
    public List<Vector3> Neighbors { get; set; }

    public HexInfo(Vector3 position, int depth, float height, HexTerrain hexTerrain, HexBuilding hexBuilding)
    {
        Position = position;
        Depth = depth;
        Height = height;
        this.hexTerrain = hexTerrain;
        this.hexBuilding = hexBuilding;
    }

    private List<HexInfo> instantiatedPositions = new List<HexInfo>();

    void InstantiateHexagon(Transform parentTransform ,Vector3 position, int depth, float height, HexTerrain hexTerrain, HexBuilding hexBuilding)
    {
        if (!PositionExistsInList(position))
        {
            Instantiate(hexTerrain.Prefab, position, Quaternion.identity, parentTransform);
            instantiatedPositions.Add(new HexInfo(position, depth, height, hexTerrain, hexBuilding));
            Debug.Log("position: " + position + ", depth: " + depth);
        }
    }

    bool PositionExistsInList(Vector3 position)
    {
        float tolerance = 0.1f; // Adjust this value as needed
        foreach (HexInfo hexInfo in instantiatedPositions)
        {
            if (Vector3.Distance(hexInfo.Position, position) < tolerance)
            {
                return true;
            }
        }
        return false;
    }

    public void InstantiateNeighbors(Vector3 center, float radius, int maxDepth, float height, Transform parentObject)
    {
        Queue<(Vector3, int)> queue = new Queue<(Vector3, int)>();
        queue.Enqueue((center, 0));

        while (queue.Count > 0)
        {
            (Vector3 hex, int depth) = queue.Dequeue();

            for (int i = 0; i < 6; i++)
            {
                float angle_deg = 60 * i + 30; // Add an offset of 30 degrees
                float angle_rad = Mathf.PI / 180 * angle_deg;
                Vector3 hexCoords = new Vector3(hex.x + radius * Mathf.Cos(angle_rad), hex.y, hex.z + radius * Mathf.Sin(angle_rad));

                if (!PositionExistsInList(hexCoords))
                {
                    InstantiateHexagon(parentObject, hexCoords, depth, 1, _grass, _none);

                    if (depth < maxDepth)
                    {
                        queue.Enqueue((hexCoords, depth + 1));
                    }
                }
            }
        }
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