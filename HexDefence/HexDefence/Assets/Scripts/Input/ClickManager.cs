using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ClickManager : MonoBehaviour
{
    [SerializeField]
    private HexGridManager _hexGridManager;

    [SerializeField]
    private GraphicRaycaster graphicRaycaster;

    [SerializeField]
    private EventSystem eventSystem;

    public ClickType[] clickTypes;
    public static event Action<HexCell, RaycastHit> OnHexSelected;
    private HexCell _currentBuildHexCell;

    private void Start()
    {
        if (HexGridManager.Instance == null)
        {
            _hexGridManager = FindObjectOfType<HexGridManager>();
            if (_hexGridManager != null)
                return;
            Debug.Log("HexGridManager not found");
            gameObject.SetActive(false);
        }
        if (EventSystem.current == null)
        {
            eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem != null)
                return;
            Debug.Log("EventSystem not found");
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        PlayerInput.Instance.OnLeftMouseClick += OnLeftMouseClick;
        PlayerInput.Instance.OnRightMouseClick += OnRightMouseClick;
    }

    private void OnDisable()
    {
        PlayerInput.Instance.OnLeftMouseClick -= OnLeftMouseClick;
        PlayerInput.Instance.OnRightMouseClick -= OnRightMouseClick;
    }

    private void Update()
    {
        if (GameManager.Instance.GamePhase != GamePhase.Defend && GameManager.Instance.Buildmode)
        {
            Buildmode();
        }
    }

    private void Buildmode()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit _hit;
        if (Physics.Raycast(ray, out _hit))
        {
            if (_hit.transform.gameObject.TryGetComponent<HexCell>(out HexCell hexCell))
            {
                if (hexCell.RoadEntryPoint == null && hexCell.HexBuilding.HexBuildingType == HexBuildingType.None && hexCell.IsTemp == false)
                {
                    if (_currentBuildHexCell != null)
                    {
                        if (hexCell != _currentBuildHexCell)
                        {
                            if (_currentBuildHexCell != null)
                            _currentBuildHexCell.RevertTempBuilding();
                            _currentBuildHexCell = hexCell;
                            _currentBuildHexCell.SetTempBuilding();
                        }
                    }
                    else
                    {
                        _currentBuildHexCell = hexCell;
                        _currentBuildHexCell.SetTempBuilding();
                    }
                }
            }
        }
    }

    private ClickType GetScriptableObjectByLayerMask(ClickType[] array, int layer)
    {
        int layerMask = 1 << layer;

        for (int i = 0; i < array.Length; i++)
        {
            if ((array[i].layerMask.value & layerMask) != 0)
            {
                return array[i];
            }
        }

        return null;
    }

    private void OnLeftMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit _hit;

        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Input.mousePosition;

        // Define the layers to hit
        int layerMask = 1 << LayerMask.NameToLayer("Tower");

        // Invert the layerMask to ignore the specified layer
        layerMask = ~layerMask;

        if (Physics.Raycast(ray, out _hit, Mathf.Infinity, layerMask))
        {
            List<RaycastResult> results = new List<RaycastResult>();

            graphicRaycaster.Raycast(pointerData, results);

            if (results.Count > 0)
            {
                OnLeftUIClick();
            }
            else
            {
                OnLeft3DClick(_hit);
            }
        }
    }

    private void OnLeftUIClick() { }

    private void OnLeft3DClick(RaycastHit _hit)
    {
        LayerMask layerMaskHit = _hit.transform.gameObject.layer;
        ClickType type = GetScriptableObjectByLayerMask(clickTypes, layerMaskHit);
        if (type == null) // ignore the layers with no Click VFX added to Clickamanager
            return;
        if (type.pooledObject == null) // ignore the layers with no Click VFX
            return;
        PooledObject obj = PooledObjectManager.Instance.Get(type.pooledObject);
        obj.transform.position = _hit.point;
        HexCell _hexCell = null;

        if (_hit.transform.gameObject.TryGetComponent<HexCell>(out HexCell hexCell))
        {
            _hexCell = hexCell;
        }
        
        switch (layerMaskHit.value)
        {
            case 4:
                OnWaterClick(_hit);
                break;
            case 7:
                OnTowerClick(_hit, _hexCell);
                break;
            case 6:
                OnLandClick(_hit, _hexCell);
                break;
            case 9:
                OnEnemyClick(_hit, _hexCell);
                break;
            case 10:
                OnTempLandClick(_hit, _hexCell);
                break;
            case 12:
                OnLandClick(_hit, _hexCell);
                break;
            default:
                break;
        }
    }

    private void OnWaterClick(RaycastHit _hit)
    {
        _hexGridManager.DeselectHexCell();
        OnHexSelected?.Invoke(null, _hit);
        GameManager.Instance.SetBuildMode(false);
        if (_currentBuildHexCell != null)
        {
            _currentBuildHexCell.RevertTempBuilding();
            _currentBuildHexCell = null;
        }
    }

    private void OnLandClick(RaycastHit _hit, HexCell hexCell)
    {
        if (GameManager.Instance.Buildmode == false)
        {
            _hexGridManager.SelectHexCell(hexCell);
            OnHexSelected?.Invoke(hexCell, _hit);
        }

        if (GameManager.Instance.GamePhase != GamePhase.Defend && GameManager.Instance.Buildmode == true)
        {
            if( Currency.Instance.GoldCurrency >= GameManager.Instance.TempBuilding.Cost)
            {
                _currentBuildHexCell.BuildHexBuilding(GameManager.Instance.TempBuilding);
                Currency.Instance.UpdateCurrency(-GameManager.Instance.TempBuilding.Cost, CurrencyType.GoldCurrency);
                if (!GameManager.Instance.MultiBuildMode)
                {
                GameManager.Instance.SetBuildMode(false, null);
                _currentBuildHexCell = null;
                }
                else
                {
                    HexBuilding tempBuilding = GameManager.Instance.TempBuilding;
                    GameManager.Instance.SetBuildMode(false, null);
                    GameManager.Instance.SetBuildMode(true, tempBuilding);
                }
            }
            else
            {
                Debug.Log("Not enough gold"); //TODO: Add UI for this
                _currentBuildHexCell.RevertTempBuilding();
                GameManager.Instance.SetBuildMode(false, null);
                _currentBuildHexCell = null;
            }

        }
    }

    private void OnTowerClick(RaycastHit _hit, HexCell hexCell)
    {
        //This needs to be ignored for now
    }

    private void OnEnemyClick(RaycastHit _hit, HexCell hexCell)
    {
        //TODO implement Call to show enemy inSelectedHexCell - Also change the name of the method to SelectedObject
    }

    private void OnTempLandClick(RaycastHit _hit, HexCell hexCell)
    {
        if (_hexGridManager.PositionExistsInList(_hexGridManager.TempHexCells, hexCell.Position))
            _hexGridManager.SelectHexCell(hexCell);
        else
            return;
    }

    private void OnRightMouseClick() { }
}
