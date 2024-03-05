using System.Collections;
using System.Collections.Generic;
using BBX.Dialogue.GUI;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] private SelectedHexCell _selectedHexCell;
    private HexCell _lastSelectedHexCell;
    private BuildingButtons _buildingButtons;
    public HexBuilding _selectedBuilding { get; private set; }
    [SerializeField]private AudioSource _audioSource;
    [SerializeField]private Currency _hexCurrency;
    [SerializeField]private LevelDisplay _levelDisplay;
    [SerializeField]private Canvas _canvas;

    private void Start()
    {

        if (_buildingButtons == null)
            _buildingButtons = FindObjectOfType<BuildingButtons>();
        else
            Debug.Log("BuildingButtons not found");
        
        if (_canvas == null)
            _canvas = this.GetComponent<Canvas>();
        else
            Debug.Log("Canvas not found");

    }

    private void OnEnable()
    {
        StartCoroutine(WaitForGameManager());
        ClickManager.OnHexSelected += OnHexSelectedUI;
    }

    private IEnumerator WaitForGameManager()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
        GameManager.Instance.OnLevelStart += SetLevel;
        GameManager.Instance.OnLevelComplete += SetLevelComplete;
        GameManager.Instance.UpdateGamePhase += UpdateGamePhase;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLevelStart -= SetLevel;
        }
        ClickManager.OnHexSelected -= OnHexSelectedUI;
        GameManager.Instance.OnLevelComplete -= SetLevel;
        GameManager.Instance.UpdateGamePhase -= UpdateGamePhase;
    }

    private void OnHexSelectedUI(HexCell hexCell, RaycastHit hit)
    {   
        Vector2 hexCellScreenPosition = RaycastResultToCanvasPosition(hit, _canvas);
        _lastSelectedHexCell = hexCell;
        _selectedHexCell.SetSelectedHexCell(hexCell, hexCellScreenPosition);
    }

    public void SetLevel(int level, Level _level)
    {
        _levelDisplay.UpdateLevel(level);
    }

    private void SetLevelComplete(int level, Level _level)
    {
        _levelDisplay.UpdateLevel(level, true);
    }

    private void UpdateGamePhase(GamePhase gamePhase)
    {
        switch (gamePhase)
        {
            case GamePhase.Income:
                _levelDisplay.UpdateGamePhaseUI(gamePhase);
                break;
            case GamePhase.HexPlacement:
                _levelDisplay.UpdateGamePhaseUI(gamePhase);
                break;
            case GamePhase.Build:
                _levelDisplay.UpdateGamePhaseUI(gamePhase);
                break;
            case GamePhase.Defend:
                break;
        }
    }

    public void SetSelectedBuilding(HexBuilding building)
    {
        _selectedBuilding = building;
        if (_lastSelectedHexCell.HexBuilding.HexBuildingType != HexBuildingType.None){
            Debug.Log("There is already a building here");
            // TODO: Show a message to the player
            return;
        }
        _audioSource.Play();
        _lastSelectedHexCell.BuildHexBuilding(_selectedBuilding);
    }

    public Vector2 RaycastResultToCanvasPosition(RaycastHit hit, Canvas canvas)
    {
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(hit.point);
        Vector2 canvasPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPosition, null, out canvasPosition);

        return canvasPosition;
    }

}
