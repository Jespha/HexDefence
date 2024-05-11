using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeRarity", menuName = "ScriptableObjects/UpgradeRarity", order = 40)]
public class UpgradeRarity : ScriptableObject
{

    public List<Color> rarityColors = new List<Color> {Color.white, Color.green, Color.blue, Color.magenta, Color.yellow};
    public Dictionary<Rarity, Color> rarityToColor;

    public void InitializeRarityToColor()
    {
        rarityToColor = new Dictionary<Rarity, Color>
        {
            {Rarity.Common, rarityColors[0]},
            {Rarity.Uncommon, rarityColors[1]},
            {Rarity.Rare, rarityColors[2]},
            {Rarity.Epic, rarityColors[3]},
            {Rarity.Legendary, rarityColors[4]}
        };
    }

}
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }