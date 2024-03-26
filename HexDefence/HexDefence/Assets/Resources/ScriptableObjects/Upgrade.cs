using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "ScriptableObjects/Upgrade", order = 20)]
public class Upgrade : ScriptableObject
{

    public UpgradeType upgradeType;


    public void SetUpgradeType()
    {

    }

}

public enum UpgradeType
{
    Tower,
    Hex,
    Enemy
}