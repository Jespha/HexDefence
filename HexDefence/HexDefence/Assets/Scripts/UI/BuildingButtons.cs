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
    [SerializeField] private Dictionary<AttackType, string> _attacktype = new Dictionary<AttackType, string>();
    
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

    // initalize dictionary _attacktype
        _attacktype.Add(AttackType.Projectile , "Projectile");
        _attacktype.Add(AttackType.Splash, "Splash");
        _attacktype.Add(AttackType.Area, "Area");
        _attacktype.Add(AttackType.Beam, "Beam");
        _attacktype.Add(AttackType.Buff, "Buff");
        _attacktype.Add(AttackType.Debuff, "Debuff");
        _attacktype.Add(AttackType.Summon, "Summon");
        _attacktype.Add(AttackType.Trap, "Trap");
        _attacktype.Add(AttackType.Turret, "Turret");

    }

    private void Update()
    {

    }

    private void BuildMode(int button)
    {
        if (_buttons.Count == button)
        {
            _buttons[button - 1].SetBuildingBuildMode();
        }
    }

    private void OnBuildMode()
    {
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
        buildingDescription[2].text = "<sprite name=\"" + _attacktype[GameManager.Instance.TempBuilding.AttackType] + "\"> " + GameManager.Instance.TempBuilding.AttackType.ToString();
        buildingDescription[3].text = "<sprite name=\"Upgrade\"> " + GameManager.upgradesUnlockedInstance.UnlockedTowerUpgradesCount(GameManager.Instance.TempBuilding).ToString();
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
