using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
    public HexBuilding _building { get; private set; }

    [SerializeField]
    private Image _image;

    [SerializeField]
    private int _buttonId;

    [SerializeField]
    private TextMeshProUGUI _hotkey;

    [SerializeField]
    private Toggle toggle;
    private UIManager _uiManager;

    public void Initialization(HexBuilding hexBuilding, UIManager uiManager, int hotkey)
    {
        _buttonId = hotkey;
        _building = hexBuilding;
        _image.sprite = hexBuilding.Icon;
        _uiManager = uiManager;
        _hotkey.text = _buttonId.ToString();
        toggle.onValueChanged.AddListener(isOn => SetBuildingBuildMode(isOn));
    }

    public void SetBuildingBuildMode(bool isOn)
    {
        // _uiManager.buildingButtons.BuildingButtonsBuildMode(_buttonId);
        bool isInPool = PooledObjectManager.Instance.IsInPool(_building.Prefab);
        if (!isInPool)
            PooledObjectManager.Instance.AddToPool(_building.Prefab, 1);

        GameManager.Instance.SetBuildMode(true, _building);
        HexGridManager.Instance.DeselectHexCell();
        toggle.animator.SetTrigger("Highlighted");
    }

    public void ResetButton()
    {
        toggle.animator.SetTrigger("Normal");
    }
}
