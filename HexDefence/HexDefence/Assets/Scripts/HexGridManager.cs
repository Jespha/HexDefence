using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class HexGridManager : MonoBehaviour
{
    [field:Header ("Base Hex Setup") ]
    [field:SerializeField] private HexCell _hexCell ;
    public static HexGridManager Instance;
    [SerializeField] public bool showCoordinates;

    [field:SerializeField] public int HexSize { get; private set; } 
    [field:SerializeField][field:Range(1, 4)] public int  StartRoads { get; private set; } = 1;
    [field:SerializeField][field:Range(0, 4)] public int  Depth { get; private set; } = 1;
    [field:SerializeField] public List<HexCell> HexCells { get; private set; } = new List<HexCell>();

    [SerializeField] private HexTerrain _hexTerrain;
    [SerializeField] private HexBuilding _hexBuilding;
    [SerializeField] private HexBuilding _baseTowerPrefab;
    [SerializeField] private HexCell _selected;

    private Vector3 _baseTowerVector;

    void Awake() => Instance = this;

    void Start()
    {
        _baseTowerVector = transform.position;
        MakeHexGrid();
    }

    public void MakeHexGrid()
    {
        ClearHexGrid();
        InstantiateHexagon(_baseTowerVector, 0, _hexTerrain, _baseTowerPrefab);
        InstantiateNeighbors(_baseTowerVector, HexSize, Depth);
    } 
    
    void InstantiateHexagon(Vector3 position, int depth)
    {
        if (!PositionExistsInList(HexCells, position))
        {
            HexCell newHexCell = Instantiate(_hexCell,this.transform);
            newHexCell.Initialize(position, depth, 1, _hexTerrain, _hexBuilding, new List<Vector3>(), this);
            newHexCell.transform.position = position;
            HexCells.Add(newHexCell);
        }
    }

    void InstantiateHexagon(Vector3 position, int depth, HexTerrain _hexTerrain, HexBuilding _hexBuilding)
    {
        if (!PositionExistsInList(HexCells, position))
        {
            HexCell newHexCell = Instantiate(_hexCell,this.transform);
            newHexCell.Initialize(position, depth, 1, _hexTerrain, _hexBuilding, new List<Vector3>(), this);
            newHexCell.transform.position = position;
            HexCells.Add(newHexCell);
        }
    }

    public bool PositionExistsInList(List<HexCell> hexCellList, Vector3 position)
    {
        foreach (var hexCell in hexCellList)
        {
            if (hexCell.Position == position)
            {
                return true;
            }
        }
        return false;
    }

    public void InstantiateNeighbors(Vector3 center, float radius, int maxDepth)
    {
        Queue<(Vector3, int)> queue = new Queue<(Vector3, int)>();
        queue.Enqueue((center, 0));

        while (queue.Count > 0)
        {
            (Vector3 hex, int depth) = queue.Dequeue();

            for (int i = 0; i < 6; i++)
            {
                float angle_deg = 60 * i;
                float angle_rad = Mathf.PI / 180 * angle_deg;
                Vector3 hexCoords = new Vector3(hex.x + radius * Mathf.Cos(angle_rad), hex.y, hex.z + radius * Mathf.Sin(angle_rad));

                if (!PositionExistsInList(HexCells,hexCoords))
                {
                    InstantiateHexagon(hexCoords, depth);

                    if (depth < maxDepth)
                    {
                        queue.Enqueue((hexCoords, depth + 1));
                    }
                }
            }
        }
    }

    public void ClearHexGrid()
    {
        HexCells.Clear();

        for (int i = this.transform.childCount; i > 0; --i)
            UnityEngine.Object.DestroyImmediate(this.transform.GetChild(0).gameObject);
    }

    public void Test()
    {
        for (int i = 0; i < HexCells.Count; i++)
        {
            HexCells[i].RandomizeHeight();
        }
    }

    public void SelectHexCell(HexCell hexCell)
    {
        if (_selected != null)
        {
            _selected.GetComponent<MeshRenderer>().material.SetFloat("_selected", 0);
        }
        
        _selected = hexCell;
        _selected.Selected();
        _selected.GetComponent<MeshRenderer>().material.SetFloat("_selected", 1);
    }

    public void DeselectHexCell()
    {
        if (_selected != null)
        {
            _selected.GetComponent<MeshRenderer>().material.SetFloat("_selected", 0);
            _selected = null;
        }
    }

}
