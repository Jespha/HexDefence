using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class SelectedHexCell : MonoBehaviour
{
    [field: SerializeField]
    public HexCell _selectedHexCell { get; private set; }

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private UnityEngine.UI.Image _hexSprite;

    [SerializeField]
    private UnityEngine.UI.Image _towerSprite;

    [SerializeField]
    private TextMeshProUGUI _name;

    [SerializeField]
    private List<TextMeshProUGUI> _stats;

    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private RectTransform _rectTransform;

    [SerializeField]
    private UILineRenderer _lineRenderer;

    [SerializeField]
    private AnimationCurve _lineCurve;

    [Header("Animation")]
    [SerializeField]
    private Vector2 baseOffset;

    [SerializeField]
    private Vector2 _rectOffset;

    [SerializeField]
    private AnimationCurve _clickCurve;

    private Vector2 zoomOffset;
    private bool isZooming = false;
    private Vector2 offset;
    private Coroutine setPositionCoroutine;
    private Coroutine setPositionCoroutineLine;

    [SerializeField]
    private float targetOffset;
    private float targetOffsetx;
    private Vector2 hexCellScreenPos;
    public Vector2 CurrentOffset;

    [SerializeField]
    private Vector2 _lineRenderPos;

    public Vector2 temp;
    public Vector2 temp2;

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
            hexCellScreenPos = AnimationCoroutine.WorldToUISpace(
                canvas,
                (_selectedHexCell.transform.position + new Vector3(0, 0, targetOffset))
            );
            _rectTransform.position = hexCellScreenPos + _rectOffset + CurrentOffset;
            _lineRenderer.Points[2] = _lineRenderPos - CurrentOffset;
            _lineRenderer.SetAllDirty();
        }

        targetOffset = math.remap(
            0,
            30,
            temp.x,
            temp.y,
            GameManager.Instance.FollowTarget.transform.position.y
        );
        targetOffsetx = math.remap(
            0,
            30,
            temp2.x,
            temp2.y,
            GameManager.Instance.FollowTarget.transform.position.y
        );
    }

    private IEnumerator WaitForPlayerInput()
    {
        yield return new WaitUntil(
            () => GameManager.Instance != null && GameManager.Instance.PlayerInput != null
        );
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
        // UnSetSelectedHexCell();
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
            hexCellScreenPos = AnimationCoroutine.WorldToUISpace(
                canvas,
                (hexCell.transform.position + new Vector3(0, 0, targetOffset))
            );
            // hexCellScreenPos = AnimationCoroutine.WorldToUISpace(canvas,hexCell.transform.position);
            _selectedHexCell = hexCell;
            _hexSprite.sprite = _selectedHexCell.HexTerrain.Sprite;
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            // Vector2 _rectOffset = hexCellScreenPos + new Vector2(50, 50);
            hexCellScreenPos = new Vector2(
                hexCellScreenPos.x + SetOffset(hexCell).x,
                hexCellScreenPos.y + SetOffset(hexCell).y
            );

            CurrentOffset = new Vector2(SetOffset(hexCell).x, SetOffset(hexCell).y);

            if (setPositionCoroutine != null)
            {
                StopCoroutine(setPositionCoroutine);
            }

            setPositionCoroutine = StartCoroutine(
                AnimationCoroutine.AnimatedVector2ToZero(
                    this,
                    CurrentOffset,
                    _curve: _clickCurve,
                    _duration: 0.5f
                )
            );

            if (setPositionCoroutineLine != null)
            {
                StopCoroutine(setPositionCoroutineLine);

                _lineRenderer.Points[1].y = _lineRenderer.Points[2].y / 2;
                _lineRenderer.Points[1].x = _lineRenderer.Points[2].x / 2;
            }

            setPositionCoroutineLine = StartCoroutine(AnimatePointPerpendicular());
        }

        if (_selectedHexCell.HexBuilding.HexBuildingType != HexBuildingType.None)
        {
            _towerSprite.sprite = _selectedHexCell.HexBuilding.Sprite;
            _towerSprite.color = Color.white;
            _name.text = _selectedHexCell.HexBuilding.Name;
            foreach (HexBuilding hexBuilding in GameManager.Instance.TowerManager.HexBuildings)
            {
                if (hexBuilding.Name == _selectedHexCell.HexBuilding.Name)
                {
                    _stats[0].text = hexBuilding.AttackDamage.ToString();
                    _stats[1].text = hexBuilding.AttackSpeed.ToString();
                    _stats[2].text = hexBuilding.AttackRange.ToString();
                }
            }
        }
        else
        {
            _towerSprite.sprite = null;
            _towerSprite.color = Color.clear;
            _name.text = _selectedHexCell.HexTerrain.Name;
        }
    }

    public void SetTempSelectedHexCell(HexCell hexCell, Vector2 hexCellScreenPosition)
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
            _hexSprite.sprite = _selectedHexCell.HexTerrain.Sprite;
            _lineRenderPos = AnimationCoroutine.WorldToUISpace(
                canvas,
                (hexCell.transform.position + new Vector3(0, 0, targetOffset))
            );
        }
    }

    public IEnumerator AnimatePointPerpendicular()
    {
        Vector2 start = _lineRenderer.Points[1];
        Vector2 direction = (_lineRenderer.Points[2] - _lineRenderer.Points[0]).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x); // 90 degrees rotation

        float duration = 0.5f;
        float distance = 10.0f;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Vector2 lineAnimation = AnimationCoroutine.Vector2LerpUnClamped(
                start,
                start * distance,
                _lineCurve.Evaluate(t)
            );
            _lineRenderer.Points[1].y = lineAnimation.y;
            _lineRenderer.Points[1].x = _lineRenderer.Points[2].x / 2;
            yield return null;
        }

        _lineRenderer.Points[1].y = _lineRenderer.Points[2].y / 2;
        _lineRenderer.Points[1].x = _lineRenderer.Points[2].x / 2;
    }

    Vector3 SetOffset(HexCell hexCell)
    {
        Vector3 _offset = Vector2.zero;

        if (Mathf.Round(hexCell.Position.x) % 2 == 0)
            _offset.z = baseOffset.y * 0.5f;
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
