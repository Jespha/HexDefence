using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SelectedHexCell : MonoBehaviour
{
    [field: SerializeField]
    public HexCell _selectedHexCell { get; private set; }

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private UnityEngine.UI.Image _hexIcon;

    [SerializeField]
    private UnityEngine.UI.Image _towerIcon;

    [SerializeField]
    private TextMeshProUGUI _name;

    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private RectTransform _rectTransform;

    [SerializeField]
    private Vector2 baseOffset;

    [SerializeField]
    private AnimationCurve _clickCurve;

    private Vector2 zoomOffset;
    private bool isZooming = false;
    private Vector2 offset;
    private Coroutine setPositionCoroutine;

    [SerializeField]private float targetOffset;
    private Vector2 hexCellScreenPos;

    private void Start()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        StartCoroutine(WaitForPlayerInput());
    }

    void Update()
    {
        if (_selectedHexCell != null)
        {
            _rectTransform.position = AnimationCoroutine.WorldToUISpace(canvas, (_selectedHexCell.transform.position + new Vector3(0, 0, targetOffset)));
        }
    }

    private IEnumerator WaitForPlayerInput()
    {
        yield return new WaitUntil(() => GameManager.Instance != null && GameManager.Instance.PlayerInput != null);
        GameManager.Instance.PlayerInput.OnScrollWheel += ZoomCameraOffset;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null && GameManager.Instance.PlayerInput != null)
        {
            GameManager.Instance.PlayerInput.OnScrollWheel -= ZoomCameraOffset;
        }
    }

    private void ZoomCameraOffset(float level)
    {
        UnSetSelectedHexCell();
    }

    public void UnSetSelectedHexCell()
    {
        _selectedHexCell = null;
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
    }

    public void SetSelectedHexCell(HexCell hexCell, Vector2 hexCellScreenPosition)
    {
        if (hexCell == null)
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            return;
        }

        if (hexCell != null)
        {
            hexCellScreenPos = AnimationCoroutine.WorldToUISpace(canvas, (hexCell.transform.position + new Vector3(0, 0, targetOffset)));
            _selectedHexCell = hexCell;
            _hexIcon.sprite = _selectedHexCell.HexTerrain.Icon;
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            // Vector2 _rectOffset = hexCellScreenPos + new Vector2(50, 50);
            Vector2 _offset = AnimationCoroutine.WorldToUISpace(canvas, (hexCell.transform.position + (SetOffset(hexCell)*3)));;
            _rectTransform.position = _offset;
            
            if (setPositionCoroutine != null)
            {
                StopCoroutine(setPositionCoroutine);
            }
            
            setPositionCoroutine = StartCoroutine(
                AnimationCoroutine.SetPositionVec2Coroutine(
                    _rectTransform,
                    hexCellScreenPos,
                    _clickCurve,
                    _duration: 0.5f
                )
            );

        }

        if (_selectedHexCell.HexBuilding.HexBuildingType != HexBuildingType.None)
        {
            _towerIcon.sprite = _selectedHexCell.HexBuilding.Icon;
            _towerIcon.color = Color.white;
            _name.text = _selectedHexCell.HexBuilding.Name;
        }
        else
        {
            _towerIcon.sprite = null;
            _towerIcon.color = Color.clear;
            _name.text = _selectedHexCell.HexTerrain.Name;
        }
    }

    Vector3 SetOffset(HexCell hexCell)
    {
        Vector3 _offset = Vector2.zero;

        if (Mathf.Round(hexCell.Position.x) % 2 == 0)
        _offset.z =  baseOffset.y * 0.5f;
        else
        {
            switch (hexCell.Position.z)
            {
                case float n when n == 0:
                    _offset.z = baseOffset.y * 1;
                    break;
                case float n when n < 0:
                    _offset.z = baseOffset.y;
                    break;
                case float n when n > 0:
                    _offset.z = 0;
                    break;
                default:
                    _offset.z = 0;
                break;
            }
        }

        if (Mathf.Round(hexCell.Position.x) % 2 == 0)
        {
            switch (hexCell.Position.x)
            {
                case float n when n < 0:
                    _offset.x = baseOffset.x;
                    break;
                case float n when n > 0:
                    _offset.x = baseOffset.x * -1;
                    break;
                default:
                    _offset.x = 0;
                    _offset.z = 1;
                break;
            }
        }
        else
        { 
            switch (hexCell.Position.x)
            {
                case float n when Mathf.Round(n) % 2 == 0:
                    _offset.x = 0;
                    break;
                case float n when n == 0:
                    _offset.x = 0;
                    break;
                case float n when n < 0:
                    _offset.x = baseOffset.x;
                    break;
                case float n when n > 0:
                    _offset.x = baseOffset.x * -1;
                    break;
                default:
                    _offset.x = 0;
                break;
            }
        }
        
        return _offset;
    
    }
}
