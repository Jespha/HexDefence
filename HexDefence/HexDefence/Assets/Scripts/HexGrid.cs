using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    [field:Header ("Base settings") ]
    [field:SerializeField] public int Width { get; private set; }
    [field:SerializeField] public int Height { get; private set; }
    [field:SerializeField] public int HexSize { get; private set; } 
    [field:SerializeField][field:Range(1, 4)] public int  Road { get; private set; } = 1;
    [field:SerializeField][field:Range(0, 4)] public int  Depth { get; private set; } = 1;

    [SerializeField] private GameObject _baseTile;
    [SerializeField] private GameObject _baseTower;
    private Vector3 _baseTowerVector;
    private List<Vector3> instantiatedPositions = new List<Vector3>();

    void Start()
    {
        _baseTowerVector = transform.position;
        MakeHexGrid();
    }

    ///<summary>Offset cordinats to fit a hexagonal grid</summary>
    private Vector2 GetHexCoords(int x, int z)
    {
        float xPos = 2 - x * HexSize * Mathf.Cos(f:Mathf.Deg2Rad * 30);
        float zPos = 2- z * HexSize + ((x % 2 == 1) ?  HexSize * 0.5f : 0);

        return new Vector2(xPos, y:zPos);

    }

    public void MakeHexGrid()
    {
        
        // ClearHexGrid();
        // Instantiate(_baseTower, _baseTowerVector, Quaternion.identity, this.transform);
        // instantiatedPositions.Add(_baseTowerVector);
        // InstantiateNeighbors(_baseTowerVector,HexSize,Depth);

    } 

    // void InstantiateHexagon(Vector3 position)
    // {
    //     if (!instantiatedPositions.Contains(position))
    //     {
    //             Instantiate(_baseTile, position, Quaternion.identity,this.transform);
    //             // Debug.Log("position: " + position);
    //             instantiatedPositions.Add(position);
    //     }
    // }

    // public void InstantiateNeighbors(Vector3 center, float radius, int maxDepth)
    // {
    //     Queue<(Vector3, int)> queue = new Queue<(Vector3, int)>();
    //     queue.Enqueue((center, 0));

    //     while (queue.Count > 0)
    //     {
    //         (Vector3 hex, int depth) = queue.Dequeue();

    //         for (int i = 0; i < 6; i++)
    //         {
    //             float angle_deg = 60 * i + 30; // Add an offset of 30 degrees
    //             float angle_rad = Mathf.PI / 180 * angle_deg;
    //             Vector3 hexCoords = new Vector3(hex.x + radius * Mathf.Cos(angle_rad), hex.y, hex.z + radius * Mathf.Sin(angle_rad));

    //             if (!instantiatedPositions.Contains(hexCoords))
    //             {
    //                 InstantiateHexagon(hexCoords);

    //                 if (depth < maxDepth)
    //                 {
    //                     queue.Enqueue((hexCoords, depth + 1));
    //                 }
    //             }
    //         }
    //     }
    // }

    // public void ClearHexGrid()
    // {
    //     instantiatedPositions.Clear();
    //     for (int i = this.transform.childCount; i > 0; --i)
    //     DestroyImmediate(this.transform.GetChild(0).gameObject);
    // }

}
