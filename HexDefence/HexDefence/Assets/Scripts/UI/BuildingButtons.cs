using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButtons : MonoBehaviour
{
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private Image _buildModeActive;

    [Header ("Building Buttons")]
    [SerializeField] private BuildingButton _buildingButtonPrefab;
    [SerializeField] private HorizontalLayoutGroup _horizontalLayoutGroup;  
    [SerializeField] private List<HexBuilding> hexBuilding;
    [SerializeField] private List<BuildingButton> _buttons = new List<BuildingButton>();

    [Header ("Building Info Panel")]
    [SerializeField] private CanvasGroup infoPanelCanvasGroup;
    [SerializeField] private RectTransform infoPanelrect;
    [SerializeField] private List<TextMeshProUGUI> buildingDescription;
    [SerializeField] private AnimationCurve infoPanelCurve;
    
    private void Start()
    {

        if (_uiManager == null)
        {
            _uiManager = FindObjectOfType<UIManager>();
            if (_uiManager == null)
            {
                Debug.Log("UIManager not found");
            }
        }

        HexBuilding[] scriptableObjects = Resources.LoadAll<HexBuilding>("");
        hexBuilding = new List<HexBuilding>(scriptableObjects);
        SetBuildingButtons();
        GameManager.Instance.PlayerInput.BuildMode += BuildMode;
        GameManager.Instance.OnBuildmode += OnBuildMode;


    }

    private void Update()
    {

    }

    private void BuildMode(int button)
    {
        if (_buttons.Count == button)
        {
            _buttons[button - 1].SetBuildingBuildMode(true);
        }
    }

    private void OnBuildMode()
    {
        if (GameManager.Instance.TempBuilding == null) //TODO: BUG Find out why Tempbuilding is null on first click
        return;

        _buildModeActive.gameObject.SetActive(GameManager.Instance.Buildmode);

        if (GameManager.Instance.Buildmode == false)
        foreach (var button in _buttons)
        {
            button.ResetButton();
        }    

        infoPanelrect.anchoredPosition = new Vector2(0, GameManager.Instance.Buildmode ? 0 : 65);
        StartCoroutine(AnimationCoroutine.SetPositionVec2Coroutine(infoPanelrect , new Vector2 (0, GameManager.Instance.Buildmode ? 65 : 0), infoPanelCurve, 0.3f));
        StartCoroutine(AnimationCoroutine.FadeCanvasGroup(0.2f, infoPanelCanvasGroup, GameManager.Instance.Buildmode ? 1 : 0));
        buildingDescription[0].text = GameManager.Instance.TempBuilding.Name.ToString();
        buildingDescription[1].text = "<sprite name=\"Gold\"> " + GameManager.Instance.TempBuilding.Cost.ToString();
        buildingDescription[2].text =  HexBuilding.AttackTypeToSprite(GameManager.Instance.TempBuilding.AttackType) + " " + GameManager.Instance.TempBuilding.AttackType.ToString();
        buildingDescription[3].text = "<sprite name=\"Upgrade\"> " + GameManager.UpgradesUnlockedInstance.UnlockedTowerUpgradesCount(GameManager.Instance.TempBuilding).ToString();
    }

    public void SetBuildingButtons()
    {
        if (_buttons.Count > 0)
        {
            foreach (var button in _buttons)
            {
                Destroy(button.gameObject);
            }
            _buttons.Clear();
        }

        foreach (HexBuilding building in hexBuilding)
        {   
            if (building.Level <= 0)
            {
                continue; // Skip this iteration and move to the next building
            }

            var button = Instantiate(_buildingButtonPrefab, _horizontalLayoutGroup.transform);
            button.Initialization(building, _uiManager);
            _buttons.Add(button);
        }
    }
}
