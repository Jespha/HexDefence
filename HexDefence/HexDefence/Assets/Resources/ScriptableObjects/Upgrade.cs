using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "ScriptableObjects/Upgrade", order = 20)]
public class Upgrade : ScriptableObject
{
    public string upgradeName;
    public string upgradeDescription;
    public Sprite upgradeIcon;
    /// <summary>    
    /// NOTE: "isUnlock" in the indevidual upgrade is profile specific not game specific        
    /// </summary>
    [Header("Is Unlocked in Profile")]
    public bool IsUnlockedInProfile;
    public Upgrade[] prerequisites;
    [EnumToggleButtons]
    public UpgradeType upgradeType;
    [EnumToggleButtons]
    public Rarity rarity; 
    
    // TODO: Add rarity modifier to the upgrade in randomizer and border color in UpgradeUI - make ita scritable object or dictionary with color and modifier

    [ShowIfGroup("TowerGroup", Condition = "@upgradeType == UpgradeType.Tower")]
    [BoxGroup("TowerGroup")]
    public HexBuilding _building;
    [BoxGroup("TowerGroup/Stats")]
    public float AttackSpeedUpgrade = 0;
    [BoxGroup("TowerGroup/Stats")]
    public float AttackRangeUpgrade = 0;
    [BoxGroup("TowerGroup/Stats")]
    public float AttackDamageUpgrade = 0;

    [ShowIfGroup("HexGroup", Condition = "@upgradeType == UpgradeType.Hex")]
    [BoxGroup("HexGroup/Show HexBox")]
    public HexTerrain _hex;

    [ShowIfGroup("EnemyGroup", Condition = "@upgradeType == UpgradeType.Enemy")]
    [BoxGroup("EnemyGroup/Show EnemyBox")]
    public Enemy _enemy;
    
    // [ShowIfGroup("upgradeType", UpgradeType.Enemy)]
    // [BoxGroup("upgradeType/Show EnemyBox")]
    // public Enemy _enemy;

    public void SetIsUnlockedInProfile(bool isUnlockedInProfile)
    {
        IsUnlockedInProfile = isUnlockedInProfile;
    }

}

public enum UpgradeType
{
    Tower,
    Hex,
    Enemy
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
