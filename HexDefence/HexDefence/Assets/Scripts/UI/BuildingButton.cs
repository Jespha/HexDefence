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
        _uiManager = uiManager;
        _button.onClick.AddListener(() => SetBuildingBuildMode());
    }

    public void SetBuildingBuildMode()
    {

        bool isInPool = PooledObjectManager.Instance.IsInPool(_building.Prefab);
        if (!isInPool)
            PooledObjectManager.Instance.AddToPool(_building.Prefab, 1);
        
        GameManager.Instance.SetBuildMode(true, _building);
        HexGridManager.Instance.DeselectHexCell();
    }
}
