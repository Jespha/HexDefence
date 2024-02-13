using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;

public class SelectedHexCell : MonoBehaviour
{

    [field:SerializeField] public HexCell _selectedHexCell { get; private set;}
    private Sprite _hexIcon; 
    private Sprite _towerIcon; 


    public void SetSelectedHexCell(HexCell hexCell)
    {
        _selectedHexCell = hexCell;
        _hexIcon = _selectedHexCell.HexTerrain.Icon;

        if (_selectedHexCell.HexBuilding.HexBuildingType != HexBuildingType.None)
        {
            _towerIcon = _selectedHexCell.HexBuilding.Icon;
        }
    }


}

