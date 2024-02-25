using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
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
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Vector2 baseOffset;
    private Vector2 offset;

    private void Start()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
    }

    public void SetSelectedHexCell(HexCell hexCell, Vector2 hexCellScreenPosition)
    {   
        if (hexCell != null)
        {
            _selectedHexCell = hexCell;
            _hexIcon.sprite = _selectedHexCell.HexTerrain.Icon;
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;

            // offset = ;

            _rectTransform.anchoredPosition = hexCellScreenPosition + SetOffset(hexCell);
        }
        else
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
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

    Vector2 SetOffset(HexCell hexCell)
    {
            Vector2 _offset;
            if (hexCell.Position.y<=0)
            _offset.y = baseOffset.y;
            else
            _offset.y = baseOffset.y * -1;

            if (hexCell.Position.x<=0)
            _offset.x = baseOffset.x * -1;
            else
            _offset.x = baseOffset.x;
            return _offset;
    }

}

