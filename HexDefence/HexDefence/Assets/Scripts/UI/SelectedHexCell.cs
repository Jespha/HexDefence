using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SelectedHexCell : MonoBehaviour
{

    [field:SerializeField] public HexCell _selectedHexCell { get; private set;}
    [SerializeField] private UnityEngine.UI.Image _hexIcon; 
    [SerializeField] private UnityEngine.UI.Image _towerIcon; 
    [SerializeField] private TextMeshProUGUI _name; 
    [SerializeField] private CanvasGroup _canvasGroup;

    public void SetSelectedHexCell(HexCell hexCell)
    {   
        if (hexCell != null)
        {
            _selectedHexCell = hexCell;
            _hexIcon.sprite = _selectedHexCell.HexTerrain.Icon;
            _canvasGroup.alpha = 1;
            // gameObject.transform.position = _selectedHexCell.Position;
        }
        else
        {
            _canvasGroup.alpha = 0;
        }

        if (_selectedHexCell.HexBuilding.HexBuildingType != HexBuildingType.None)
        {
            _towerIcon.sprite = _selectedHexCell.HexBuilding.Icon;
            _towerIcon.color = Color.white;
            _name.text = _selectedHexCell.HexBuilding.Name.ToString();
        }
        else
        {
            _towerIcon.sprite = null;
            _towerIcon.color = Color.clear;
            _name.text = _selectedHexCell.HexTerrain.HexTerrainType.ToString();
        }
    }


}

