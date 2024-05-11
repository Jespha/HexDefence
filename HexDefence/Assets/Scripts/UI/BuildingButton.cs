using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{

    public HexBuilding _building { get; private set; }
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private Toggle toggle;
    private UIManager _uiManager;

    public void Initialization(HexBuilding hexBuilding, UIManager uiManager)
    {
        _building = hexBuilding;
        _image.sprite = hexBuilding.Icon;
        _uiManager = uiManager;
        toggle.onValueChanged.AddListener(isOn => SetBuildingBuildMode(isOn));
    }

    public void SetBuildingBuildMode(bool isOn)
    {

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