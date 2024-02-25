using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  HexGridManager class is responsible for creating and managing the hex grid
/// </summary>
public class HexGridManager : MonoBehaviour
{
    [field:Header ("Base Hex Setup") ]
    [field:SerializeField] private HexCell _hexCell;
    [field:SerializeField] private HexCell _hexCellTemp;
    public static HexGridManager Instance;
    [SerializeField] public bool showCoordinates;

    [field:SerializeField] public int HexSize { get; private set; } 
    [field:SerializeField][field:Range(1, 6)] public int  StartRoads { get; private set; } = 1;
    [field:SerializeField][field:Range(0, 4)] public int  Depth { get; private set; } = 1;
    [field:SerializeField] public List<HexCell> HexCells { get; private set; } = new List<HexCell>();
    [field:SerializeField] public List<HexCell> TempHexCells { get; private set; } = new List<HexCell>();
    [field:SerializeField] public HexCell[,] HexCells2DArray;

    [SerializeField] private HexTerrain _hexTerrain;
    [SerializeField] private HexBuilding _hexBuilding;
    [SerializeField] private HexBuilding _baseTowerPrefab;
    [SerializeField] private HexCell _selected;
    [SerializeField] private HexCell _selectedTemp;

    void Awake() => Instance = this;

    void Start()
    {
        MakeHexGrid();
    }

    public void MakeHexGrid()
    {
        ClearHexGrid();
        RoadManager.Instance.ClearRoads();
        InstantiateHexagon(new Vector3(0,0,0), -1, _hexTerrain, _baseTowerPrefab);
        InitializeRoads(HexCells[0]);
        InstantiateNeighbors(new Vector3(0,0,0), HexSize, Depth);
    } 

    public void InstantiateTempHexagon(Vector3 position, int depth)
    {
        HexCell newHexCell = Instantiate(_hexCellTemp,this.transform);
        newHexCell.Initialize(position, depth, 1, _hexTerrain, _hexBuilding, new List<Vector3>(), this);
        newHexCell.InitializationTemp(true, TempHexCells.Count);
        newHexCell.transform.position = position;
        TempHexCells.Add(newHexCell);
    }

    public void ClearTempHexGrid()
    {
        for (int i = TempHexCells.Count - 1; i >= 0; --i)
        {
            DestroyImmediate(TempHexCells[i].gameObject,true);
        }
        TempHexCells.Clear();
    }

    void InstantiateHexagon(Vector3 position, int depth, List<Vector3> neighborPositions)
    {
        if (!PositionExistsInList(HexCells, position))
        {
            HexCell newHexCell = Instantiate(_hexCell,this.transform);
            newHexCell.Initialize(position, depth, 1, _hexTerrain, _hexBuilding, neighborPositions, this);
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

    HexCell InstantiateHexagonFromTemp(HexCell hexcell, HexCell hexcellParent)
    {
        HexCell newHexCell = Instantiate(_hexCell,this.transform);
        newHexCell.Initialize(hexcell.Position, hexcell.Depth, 1, hexcell.HexTerrain, hexcell.HexBuilding, new List<Vector3>(), this);
        newHexCell.transform.position = hexcell.Position;
        HexCells.Add(newHexCell);
        if (hexcellParent.RoadEntryPoint != null && hexcellParent.RoadEndPoint == null)
        {
            RoadManager.Instance.AddRoad(newHexCell,hexcellParent);
            hexcellParent.SetRoad(newHexCell, RoadType.Exit);
            newHexCell.SetRoad(hexcellParent, RoadType.Entry, hexcellParent.RoadIndex);
            // AddRoadToArray(newHexCell, hexcellParent.RoadIndex);
        }
        return newHexCell;
    }

    void InstantiateHexagonRoad(Vector3 position, int depth, List<Vector3> neighborPositions, HexCell hexCell, int roadIndex)
    {
        if (!PositionExistsInList(HexCells, position))
        {
            HexCell newHexCell = Instantiate(_hexCell,this.transform);
            newHexCell.Initialize(position, depth, 1, _hexTerrain, _hexBuilding, neighborPositions, this);
            newHexCell.SetRoad(hexCell, RoadType.Entry, roadIndex);
            newHexCell.transform.position = position;
            RoadManager.Instance.CreateRoad(hexCell , newHexCell, roadIndex);
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

        int _startRoads = StartRoads;

        while (queue.Count > 0)
        {
            (Vector3 hex, int depth) = queue.Dequeue();

            for (int i = 0; i < 6; i++)
            {
                float angle_deg = 60 * i ;
                float angle_rad = Mathf.PI / 180 * angle_deg;
                Vector3 hexCoords = new Vector3(
                    hex.x + radius * Mathf.Cos(angle_rad), 
                    hex.y, 
                    hex.z + radius * Mathf.Sin(angle_rad));
                    

                if (!PositionExistsInList(HexCells,hexCoords))
                {
                    List<Vector3> neighborPositions = new List<Vector3>();

                    // Calculate the positions of the neighbors
                    for (int j = 0; j < 6; j++)
                    {
                        float neighbor_angle_deg = 60 * j;
                        float neighbor_angle_rad = Mathf.PI / 180 * neighbor_angle_deg;
                        Vector3 neighborCoords = new Vector3(
                            hexCoords.x + radius * Mathf.Cos(neighbor_angle_rad), 
                            hexCoords.y, 
                            hexCoords.z + radius * Mathf.Sin(neighbor_angle_rad));

                        neighborPositions.Add(neighborCoords);
                    }

                    if (depth == 0)
                    {                           
                
                        if (6 - HexCells.Count <= _startRoads - 1)
                        {
                            InstantiateHexagonRoad(hexCoords, depth, neighborPositions, HexCells[0], _startRoads);
                            _startRoads = _startRoads - 1;
                        }
                        else
                        {
                            bool _road = UnityEngine.Random.value > 0.5f;
                            if (_road && _startRoads > 0)
                            {
                                InstantiateHexagonRoad(hexCoords, depth, neighborPositions, HexCells[0], _startRoads);
                                _startRoads = _startRoads - 1;
                            }
                            else
                            {
                                InstantiateHexagon(hexCoords, depth, neighborPositions);
                            }
                        }
                    }
                    else
                    {
                        InstantiateHexagon(hexCoords, depth, neighborPositions);
                    }

                    if (depth < maxDepth)
                    {
                        queue.Enqueue((hexCoords, depth + 1));
                    }
                }
            }
        }
    }

    HexCell InstantiateHexagonNeighborsFromTemp(HexCell hexCell, HexCell hexcellParent)
    {


        List<Vector3> neighborPositions = new List<Vector3>();

        // Calculate the positions of the neighbors
        for (int j = 0; j < 6; j++)
        {
            float neighbor_angle_deg = 60 * j;
            float neighbor_angle_rad = Mathf.PI / 180 * neighbor_angle_deg;
            Vector3 neighborCoords = new Vector3(
                hexCell.Position.x + HexSize * Mathf.Cos(neighbor_angle_rad), 
                hexCell.Position.y, 
                hexCell.Position.z + HexSize * Mathf.Sin(neighbor_angle_rad));

            neighborPositions.Add(neighborCoords);
        }

        HexCell newHexCell = Instantiate(_hexCell,this.transform);
        newHexCell.Initialize(hexCell.Position, hexCell.Depth, 1, hexCell.HexTerrain, hexCell.HexBuilding, neighborPositions, this);
        newHexCell.transform.position = hexCell.Position;
        HexCells.Add(newHexCell);
        if (hexcellParent.RoadEntryPoint != null && hexcellParent.RoadEndPoint == null)
        {
            RoadManager.Instance.AddRoad(newHexCell,hexcellParent);
            hexcellParent.SetRoad(newHexCell, RoadType.Exit);
            newHexCell.SetRoad(hexcellParent, RoadType.Entry, hexcellParent.RoadIndex);
            // AddRoadToArray(newHexCell, hexcellParent.RoadIndex);
        }

        return newHexCell;

    }

    public void ClearHexGrid()
    {
        HexCells.Clear();
        for (int i = this.transform.childCount; i > 0; --i)
            UnityEngine.Object.DestroyImmediate(this.transform.GetChild(0).gameObject);
    }

    /// <summary> Just a Test Void That Sets the Height of the HexCells to a Random Value </summary>
    public void Test()
    {
        for (int i = 0; i < HexCells.Count; i++)
        {
            HexCells[i].RandomizeHeight();
        }
    }

    public void InitializeRoads(HexCell hexCell)
    {
        HexCells2DArray = new HexCell[StartRoads, HexCells.Count];
        for (int i = 0; i < StartRoads; i++)
        {
            HexCells2DArray[i, 0] = hexCell;
        }
    }
    
private void AddRoadToArray(HexCell hexCell, int roadIndex)
{   
    Debug.Log("RoadIndex: " + roadIndex);
    int _lastRoad = HexCells2DArray.GetLength(roadIndex);
    Debug.Log("_lastRoad: " + _lastRoad);

    HexCells2DArray[roadIndex - 1, _lastRoad] = hexCell;
}
    /// <summary> Sets the selected HexCell as _selected in HexGridManager</summary>
    /// <param name="hexCell">The HexCell to be selected</param>
    public void SelectHexCell(HexCell hexCell)
    {
        if (hexCell.IsTemp)
        { 
            if ( _selectedTemp != null)
            {
                if(_selectedTemp.Position == hexCell.Position )
                {
                    if (Currency.Instance.HexCurrency >= 1)
                    {
                        HexCell newHexCell = InstantiateHexagonNeighborsFromTemp(hexCell,_selected);
                        _selectedTemp.Deselected();
                        _selected.Deselected();
                        ClearTempHexGrid();
                        _selected = newHexCell;
                        Currency.Instance.UpdateCurrency(-1, CurrencyType.HexCurrency);
                    }
                    else
                    {
                        Currency.Instance.Notify("Not enough HexCurrency");
                        //TODO: Show a message to the player. Make a notification system
                    }
                }
                else
                {
                    _selectedTemp.Deselected();
                    _selectedTemp = hexCell;
                    _selectedTemp.Selected();
                }
            }
            else
            {
                _selectedTemp = hexCell;
                _selectedTemp.Selected();
            }
        }
        else
        {       
            if (_selected != null)
            {
                _selected.Deselected();
                ClearTempHexGrid();
            }
        
            _selected = hexCell;
            _selected.Selected();
        }
    }

    public void DeselectHexCell()
    {
        if (_selected != null)
        {
            if (_selectedTemp != null)
            {
                _selectedTemp.Deselected();
                _selectedTemp = null;
            }
            _selected.Deselected();
            _selectedTemp = null;
            ClearTempHexGrid();
        }
    }
}
