using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HexTypePrefab", menuName = "ScriptableObjects/HexTypePrefab", order = 1)]
public class HexTypePrefab : ScriptableObject
{
    public string HexType;
    public GameObject Prefab;
    public List<HexTypePrefab> Neighbors;
}

public class HexInfo
{
    private HexType grass;
    private int v;

    public Vector3 Position { get; set; }
    public int Depth { get; set; }
    public string HexType { get; set; }
    public float Height { get; set; }

    public HexInfo(Vector3 position, int depth, string hexType, float height)
    {
        Position = position;
        Depth = depth;
        HexType = hexType;
        Height = height;
    }

    public HexInfo(Vector3 position, int depth, HexType grass, int v)
    {
        Position = position;
        Depth = depth;
        this.grass = grass;
        this.v = v;
    }
}
//     private List<HexInfo> instantiatedPositions = new List<HexInfo>();

//     void InstantiateHexagon(Vector3 position, int depth, HexTypePrefab hexTypePrefab, float height)
//     {
//         if (!PositionExistsInList(position))
//         {
//             Instantiate(hexTypePrefab.Prefab, position, Quaternion.identity, this.transform);
//             Debug.Log("position: " + position + ", depth: " + depth + ", hexType: " + hexTypePrefab.HexType + ", height: " + height);
//             instantiatedPositions.Add(new HexInfo(position, depth, hexTypePrefab, height));
//         }
//     }

//     bool PositionExistsInList(Vector3 position)
//     {
//         float tolerance = 0.1f; // Adjust this value as needed
//         foreach (HexInfo hexInfo in instantiatedPositions)
//         {
//             if (Vector3.Distance(hexInfo.Position, position) < tolerance)
//             {
//                 return true;
//             }
//         }
//         return false;
//     }

//     public void InstantiateNeighbors(Vector3 center, float radius, int maxDepth, string hexType, float height, GameObject parentObject)
//     {
//         Queue<(Vector3, int)> queue = new Queue<(Vector3, int)>();
//         queue.Enqueue((center, 0));

//         while (queue.Count > 0)
//         {
//             (Vector3 hex, int depth) = queue.Dequeue();

//             for (int i = 0; i < 6; i++)
//             {
//                 float angle_deg = 60 * i + 30; // Add an offset of 30 degrees
//                 float angle_rad = Mathf.PI / 180 * angle_deg;
//                 Vector3 hexCoords = new Vector3(hex.x + radius * Mathf.Cos(angle_rad), hex.y, hex.z + radius * Mathf.Sin(angle_rad));

//                 if (!PositionExistsInList(hexCoords))
//                 {
//                     InstantiateHexagon(hexCoords, depth, hexTypePrefab, height, parentObject);

//                     if (depth < maxDepth)
//                     {
//                         queue.Enqueue((hexCoords, depth + 1));
//                     }
//                 }
//             }
//         }
//     }

//     public void ClearHexGrid(GameObject parentObject)
//     {
//         instantiatedPositions.Clear();
//         for (int i = parentObject.transform.childCount; i > 0; --i)
//             UnityEngine.Object.DestroyImmediate(parentObject.transform.GetChild(0).gameObject);
//     }

// }
    public enum HexType
    {
        Grass,

    }