using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButtons : MonoBehaviour
{

    [SerializeField] private BuildingButton _buildingButton;
    [SerializeField] private HorizontalLayoutGroup _horizontalLayoutGroup;  
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private Image _buildModeImage;

    [SerializeField] private List<HexBuilding> hexBuilding;
    [SerializeField] private List<BuildingButton> _buttons = new List<BuildingButton>();

    private void Start()
    {
        if (_uiManager == null)
            _uiManager = FindObjectOfType<UIManager>();
        else
            Debug.Log("UIManager not found");

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
            _buttons[button - 1].SetBuildingBuildMode();
        }
    }

    private void OnBuildMode()
    {
        _buildModeImage.gameObject.SetActive(GameManager.Instance.Buildmode);
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

            var button = Instantiate(_buildingButton, _horizontalLayoutGroup.transform);
            button.Initialization(building, _uiManager);
            _buttons.Add(button);
        }
    }
}
