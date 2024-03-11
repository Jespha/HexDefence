using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SelectedHexCell : MonoBehaviour
{
    [field: SerializeField]
    public HexCell _selectedHexCell { get; private set; }

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

    private void Start()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        StartCoroutine(WaitForPlayerInput());
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

            _selectedHexCell = hexCell;
            _hexIcon.sprite = _selectedHexCell.HexTerrain.Icon;
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            _rectTransform.anchoredPosition = hexCellScreenPosition;
            StartCoroutine(
                AnimationCoroutine.SetPositionVec2Coroutine(
                    _rectTransform,
                    hexCell,
                    SetOffset(hexCell),
                    _clickCurve
                )
            );

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
        Vector2 _offset = Vector2.zero;

        switch (hexCell.Position.y)
        {
            case float n when n == 0:
                _offset.y = baseOffset.y * 1;
                break;
            case float n when n < 0:
                _offset.y = baseOffset.y;
                break;
            case float n when n > 0:
                _offset.y = baseOffset.y * -1;
                break;
            default:
                _offset.y = baseOffset.y * 1;
            break;
        }

        switch (hexCell.Position.x)
        {
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

        return _offset;
    }
}
