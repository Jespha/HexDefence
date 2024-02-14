using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] private SelectedHexCell _selectedHexCell;
    private HexCell _lastSelectedHexCell;
    private BuildingButtons _buildingButtons;
    public HexBuilding _selectedBuilding { get; private set; }
    [SerializeField]private AudioSource _audioSource;

    private void Start()
    {
        if (_selectedHexCell == null)
            _selectedHexCell = FindObjectOfType<SelectedHexCell>();
        else
            Debug.Log("SelectedHexCell not found");

        if (_buildingButtons == null)
            _buildingButtons = FindObjectOfType<BuildingButtons>();
        else
            Debug.Log("BuildingButtons not found");
    }

    private void OnEnable()
    {
        ClickManager.OnHexSelected += OnHexSelected;
    }

    private void OnDisable()
    {
        ClickManager.OnHexSelected -= OnHexSelected;
    }

    private void OnHexSelected(HexCell hexCell)
    {   
        _lastSelectedHexCell = hexCell;
        _selectedHexCell.SetSelectedHexCell(hexCell);
    }

    public void SetSelectedBuilding(HexBuilding building)
    {
        _selectedBuilding = building;
        if (_lastSelectedHexCell.HexBuilding.HexBuildingType != HexBuildingType.None){
            Debug.Log("There is already a building here");
            // TODO: Show a message to the player
            return;
        }
        _audioSource.Play();
        _lastSelectedHexCell.BuildHexBuilding(_selectedBuilding);
    }

}
