
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{

    [SerializeField] private ToggleGroup upgradeToggleGroup;
    [SerializeField] private List<Upgrade> upgrades;
    [SerializeField] private List<Upgrade> upgradesToChooseFrom;
    [SerializeField] private UpgradeCard upgradeCard;
    [SerializeField] private Button confirmButton;
    // [SerializeField] private CurrentUpgradesUnlocked CurrentUpgradesUnlocked;
    [Header("Animation")]
    [SerializeField] private RectTransform animationParent;
    [SerializeField] private Vector2 animaitonOffset;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private CanvasGroup canvasGroup;
    private Upgrade selectedUpgrade;
    private TaskCompletionSource<bool> tcs;

    private int upgradesToAdd = 0;
    private int upgradeAmountToChooseFrom = 3;

    private void Start()
    {
        confirmButton.interactable = false;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0;
        
    }

    [Button("Add Upgrades")]
    public async Task AddUpgradeAsync()
    {   
        tcs = null;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        ClearUpgrades();
        upgradesToAdd = upgradeAmountToChooseFrom;
        upgradesToChooseFrom = GameManager.UpgradesUnlockedInstance.CurrentUpgradesPossibilities;

        for (int i = 0; i < upgradesToAdd; i++)
        {
            int randomUpgradeIndex = UnityEngine.Random.Range(0, upgradesToChooseFrom.Count);
            Upgrade upgrade = GameManager.UpgradesUnlockedInstance.CurrentUpgradesPossibilities[randomUpgradeIndex];
            upgradesToChooseFrom.RemoveAt(randomUpgradeIndex);
            upgrades.Add(upgrade);
            UpgradeCard upgradeCardtoAdd = Instantiate(upgradeCard, upgradeToggleGroup.transform);
            Toggle upgradeCardtoAddToggle = upgradeCardtoAdd.GetComponent<Toggle>();
            upgradeCardtoAddToggle.group = upgradeToggleGroup;
            upgradeCardtoAdd.SetUpgrade(upgrade, this);
        }

        StartCoroutine(
        AnimationCoroutine.SetPositionVec2Coroutine(animationParent, animaitonOffset, curve, duration)
        );

        StartCoroutine(
        AnimationCoroutine.SetScaleVec2Coroutine(animationParent, new Vector2( 0.75f, 0.75f), new Vector2( 1f, 1f) , curve, duration)
        );

        StartCoroutine(
        AnimationCoroutine.FadeCanvasGroup(duration, canvasGroup, 1)
        );  

        confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        tcs = new TaskCompletionSource<bool>();

        await tcs.Task;
        confirmButton.interactable = false;
        confirmButton.onClick.RemoveListener(OnConfirmButtonClicked);
    }

    [Button("Queue up all Upgrades")]
    public async void AddMultipleUpgradesAsync(int numberOfUpgrades)
    {
        for (int i = 0; i < numberOfUpgrades; i++)
        {
            await AddUpgradeAsync();
        }

        OnCloseUpgradeWindow();
    }

    private void OnConfirmButtonClicked()
    {

        if (selectedUpgrade != null)
        {
            GameManager.UpgradesUnlockedInstance.AddUpgrade(selectedUpgrade);
        }

        tcs?.SetResult(true);
    }

    private void ClearUpgrades()
    {
        foreach (Transform child in upgradeToggleGroup.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnUpgradeSelected(bool _selected, Upgrade _upgrade)
    {
        confirmButton.interactable = _selected;
        selectedUpgrade = _upgrade;
    }

    [Button("Confirm Upgrade")]
    public void OnCloseUpgradeWindow()
    {
        confirmButton.interactable = false;
        canvasGroup.blocksRaycasts = false;
        StartCoroutine(
        AnimationCoroutine.SetPositionVec2Coroutine(animationParent, -animaitonOffset, curve, duration, 0.5f)
        );
        StartCoroutine(
        AnimationCoroutine.FadeCanvasGroup(duration, canvasGroup, 0f, 0.5f)
        );  

        if (GameManager.Instance.GamePhase == GamePhase.SelectUpgrade)
        {
            GameManager.Instance.SetGamePhase(GamePhase.HexPlacement);
        }
    }

}
