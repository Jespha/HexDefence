using System.Collections;
using UnityEngine;
using FMODUnity;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private SelectedHexCell selectedHexCell;
    private HexCell lastSelectedHexCell;

    [SerializeField]
    public BuildingButtons buildingButtons;
    public HexBuilding selectedBuilding { get; private set; }

    [SerializeField]
    private Currency hexCurrency;

    public LevelDisplay levelDisplay;

    [SerializeField]
    private UpgradeUI upgradeUI;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField] private EventReference buildSound;
    
    private void Awake()
    {
        _canvasGroup.alpha = 0;
    }

    private void Start()
    {
        if (buildingButtons == null)
        {
            buildingButtons = FindObjectOfType<BuildingButtons>();
            if (buildingButtons == null)
            {
                Debug.Log("BuildingButtons not found");
            }
        }

        if (canvas == null)
        {
            canvas = this.GetComponent<Canvas>();
            if (canvas == null)
            {
                Debug.Log("Canvas not found");
            }
        }

        if (upgradeUI == null)
        {
            upgradeUI = FindObjectOfType<UpgradeUI>();
            if (upgradeUI == null)
            {
                Debug.Log("UpgradeUI not found");
            }
        }
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
        GameManager.Instance.OnStartGame += StartGameUI;
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
        GameManager.Instance.OnStartGame -= StartGameUI;
    }

    private void OnHexSelectedUI(HexCell hexCell, RaycastHit hit)
    {
        Vector2 hexCellScreenPosition = RaycastResultToCanvasPosition(hit, canvas);
        lastSelectedHexCell = hexCell;
        if (!hexCell)
        {
            selectedHexCell.UnSetSelectedHexCell();
            return;
        }
        if (hexCell.IsTemp)
        selectedHexCell.SetTempSelectedHexCell(hexCell, hexCellScreenPosition);
        else
        selectedHexCell.SetSelectedHexCell(hexCell, hexCellScreenPosition);
    }

    public void SetLevel(int level, Level _level)
    {
        levelDisplay.UpdateLevel(level);
    }

    private void StartGameUI()
    {
        _canvasGroup.alpha = 1;
    }

    private void SetLevelComplete(int level, Level _level)
    {
        levelDisplay.UpdateLevel(level, true);
    }

    private void UpdateGamePhase(GamePhase gamePhase)
    {
        switch (gamePhase)
        {
            case GamePhase.Income:
                levelDisplay.UpdateGamePhaseUI(gamePhase);
                break;
            case GamePhase.SelectUpgrade:
                upgradeUI.AddMultipleUpgradesAsync(GameManager.Instance.UpgradesToAdd);
                break;
            case GamePhase.HexPlacement:
                levelDisplay.UpdateGamePhaseUI(gamePhase);
                break;
            case GamePhase.Build:
                levelDisplay.UpdateGamePhaseUI(gamePhase);
                break;
            case GamePhase.Defend:
                break;
        }
    }

    public void SetSelectedBuilding(HexBuilding building)
    {
        selectedBuilding = building;
        if (lastSelectedHexCell.HexBuilding.HexBuildingType != HexBuildingType.None)
        {
            Debug.Log("There is already a building here");
            // TODO: Show a message to the player
            return;
        }
        AudioManager.instance.PlayOneShot(buildSound, this.transform.position);
        lastSelectedHexCell.BuildHexBuilding(selectedBuilding);
    }

    public Vector2 RaycastResultToCanvasPosition(RaycastHit hit, Canvas canvas)
    {
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(hit.point);
        Vector2 canvasPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPosition,
            null,
            out canvasPosition
        );

        return canvasPosition;
    }
}
