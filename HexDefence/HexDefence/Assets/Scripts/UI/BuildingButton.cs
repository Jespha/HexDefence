using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{

    public HexBuilding _building { get; private set; }
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private Button _button;
    private UIManager _uiManager;

    public void Initialization(HexBuilding hexBuilding, UIManager uiManager)
    {
        _building = hexBuilding;
        _image.sprite = hexBuilding.Icon;
        _name.text = hexBuilding.Name;
        _uiManager = uiManager;
        _button.onClick.AddListener(() => _uiManager.SetSelectedBuilding(hexBuilding));
    }

}
