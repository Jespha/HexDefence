using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    [Header("Parent")]
    [SerializeField] private UpgradeUI upgradeUI;
    [Header("Local Variables")]
    [SerializeField] public Upgrade upgrade;
    [SerializeField] private Image upgradeIcon;
    [SerializeField] private TextMeshProUGUI upgradeName;
    [SerializeField] private TextMeshProUGUI upgradeDescription;
    [SerializeField] private Toggle toggle;
    private bool isSelected = false;
    [Header("Intro Animation")]
    [SerializeField] private RectTransform animationParent;
    [SerializeField] private Vector2 updateOffset;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float duration = 0.5f;
    [Header("Loop Animation")]
    [SerializeField] private float loopSpeed;
    [SerializeField] private float MaxRotation;
    [SerializeField] private AnimationCurve loopCurve;

    private void Awake()
    {
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
        StartCoroutine(AnimationCoroutine.IdleFloatRotate(animationParent,loopSpeed,MaxRotation,loopCurve));
    }
    

    public void SetUpgrade(Upgrade upgrade, UpgradeUI upgradeUI)
    {
        StartCoroutine(
        AnimationCoroutine.SetPositionVec2Coroutine(animationParent, updateOffset, curve, duration)
        );
        this.upgradeUI = upgradeUI;
        this.upgrade = upgrade;
        upgradeIcon.sprite = upgrade.upgradeIcon;
        upgradeName.text = upgrade.upgradeName;
        upgradeDescription.text = upgrade.upgradeDescription;
    }

    public void OnToggleValueChanged(bool value)
    {
        if (value == true)
        {
            // if (isSelected == true && value == true)
            // {
            //     StopCoroutine(AnimationCoroutine.IdleFloatRotate(animationParent,loopSpeed,MaxRotation,loopCurve));
            //     upgradeUI.TriggerOnUpgradeConfirmed(upgrade);
            // }
            isSelected = true;
            upgradeUI.OnUpgradeSelected(true, upgrade);
        }
        else if (value == false)
        {
            isSelected = false;
            upgradeUI.OnUpgradeSelected(false, upgrade);
        }
    }

}
