using System;
using System.Collections.Generic;
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
        RaycastHit hit;

        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Input.mousePosition;

        // Define the layers to hit
        int layerMask = 1 << LayerMask.NameToLayer("Tower");

        // Invert the layerMask to ignore the specified layer
        layerMask = ~layerMask;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            List<RaycastResult> results = new List<RaycastResult>();

            graphicRaycaster.Raycast(pointerData, results);

            if (results.Count > 0)
            {
                OnLeftUIClick();
            }
            else
            {
                OnLeft3DClick(hit);
            }
        }
    }

    private void OnLeftUIClick() { }

    private void OnLeft3DClick(RaycastHit hit)
    {
        LayerMask layerMaskHit = hit.transform.gameObject.layer;
        ClickType type = GetScriptableObjectByLayerMask(clickTypes, layerMaskHit);
        if (type == null) // ignore the layers with no Click VFX added to Clickamanager
            return;
        if (type.pooledObject == null) // ignore the layers with no Click VFX
            return;
        PooledObject obj = PooledObjectManager.Instance.Get(type.pooledObject);
        obj.transform.position = hit.point;
        HexCell _hexCell = null;

        if (hit.transform.gameObject.TryGetComponent<HexCell>(out HexCell hexCell))
        {
            _hexCell = hexCell;
        }

        switch (layerMaskHit.value)
        {
            case 4:
                OnWaterClick(hit);
                break;
            case 7:
                OnTowerClick(hit, _hexCell);
                break;
            case 6:
                OnLandClick(hit, _hexCell);
                break;
            case 9:
                OnEnemyClick(hit, _hexCell);
                break;
            case 10:
                OnTempLandClick(hit, _hexCell);
                break;
            case 12:
                OnLandClick(hit, _hexCell);
                break;
            default:
                break;
        }
    }

    private void OnWaterClick(RaycastHit hit)
    {
        _hexGridManager.DeselectHexCell();
        OnHexSelected?.Invoke(null, hit);
    }

    private void OnLandClick(RaycastHit hit, HexCell hexCell)
    {
        _hexGridManager.SelectHexCell(hexCell);
        OnHexSelected?.Invoke(hexCell, hit);
    }

    private void OnTowerClick(RaycastHit hit, HexCell hexCell)
    {
        //This needs to be ignored for now
    }

    private void OnEnemyClick(RaycastHit hit, HexCell hexCell)
    {
        //TODO implement Call to show enemy inSelectedHexCell - Also change the name of the method to SelectedObject
    }

    private void OnTempLandClick(RaycastHit hit, HexCell hexCell)
    {
        if (_hexGridManager.PositionExistsInList(_hexGridManager.TempHexCells, hexCell.Position))
            _hexGridManager.SelectHexCell(hexCell);
        else
            return;
    }

    private void OnRightMouseClick() { }
}
