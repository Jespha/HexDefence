using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickManager : MonoBehaviour
{
    [SerializeField] private ClickPool _pool;
    [SerializeField] private HexGridManager _hexGridManager;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private EventSystem eventSystem;

    public ClickType[] clickTypes;
    public static event Action<HexCell> OnHexSelected;
    
    private void Start()
    {
        if (_pool == null){
        
            _pool = FindObjectOfType<ClickPool>();

            if (_pool == null){
                Debug.Log("ClickPool not found");
                gameObject.SetActive(false);
            }
        } 
        if (HexGridManager.Instance == null){
            _hexGridManager = FindObjectOfType<HexGridManager>();
            if (_hexGridManager != null)
            return;
            Debug.Log("HexGridManager not found");
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
       MouseController.Instance.OnLeftMouseClick += OnLeftMouseClick;
       MouseController.Instance.OnRightMouseClick += OnRightMouseClick;
    }

    private void OnDisable()
    {
       MouseController.Instance.OnLeftMouseClick -= OnLeftMouseClick;
       MouseController.Instance.OnRightMouseClick -= OnRightMouseClick;
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

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
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
        // PointerEventData pointerData = new PointerEventData(eventSystem);
        // pointerData.position = Input.mousePosition;

        // List<RaycastResult> results = new List<RaycastResult>();

        // graphicRaycaster.Raycast(pointerData, results);

        // if (results.Count > 0)
        // {
        //     OnLeftUIClick();
        // }
        // else
        // {
        //     OnLeft3DClick(hit);
        // }        

    }

    private void OnLeftUIClick()
    {
        Debug.Log("UIClick");
    }

    private void OnLeft3DClick(RaycastHit hit)
    {
        LayerMask layerMaskHit = hit.transform.gameObject.layer;
        ClickType type = GetScriptableObjectByLayerMask(clickTypes, layerMaskHit);
        PooledObject obj = _pool.Get(type.pooledObject);
        obj.transform.position = hit.point;

        if (hit.transform.gameObject.TryGetComponent<HexCell>(out HexCell hexCell))
        {
            _hexGridManager.SelectHexCell(hexCell);
            OnHexSelected?.Invoke(hexCell);

        }
        else
        {
            _hexGridManager.DeselectHexCell();
            OnHexSelected?.Invoke(null);
        }     
        Debug.Log("3DClick");
    }

    private void OnRightMouseClick()
    {
        Debug.Log("Rclick");
    }

}
