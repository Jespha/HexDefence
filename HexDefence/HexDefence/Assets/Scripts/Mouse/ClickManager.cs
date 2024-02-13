using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    [SerializeField] private ClickPool _pool;
    [SerializeField] private HexGridManager _hexGridManager;

    public ClickType[] clickTypes;

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

    private void OnLeftMouseClick(RaycastHit hit)
    {
        LayerMask layerMaskHit = hit.transform.gameObject.layer;
        ClickType type = GetScriptableObjectByLayerMask(clickTypes, layerMaskHit);
        PooledObject obj = _pool.Get(type.pooledObject);
        obj.transform.position = hit.point;

        if (hit.transform.gameObject.TryGetComponent<HexCell>(out HexCell hexCell))
        {
            _hexGridManager.SelectHexCell(hexCell);
        }
    }

    private void OnRightMouseClick(RaycastHit hit)
    {
        Debug.Log("Rclick");
    }

}
