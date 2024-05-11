using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level_", menuName = "ScriptableObjects/Level", order = 8)]
public class Level : ScriptableObject
{

    [System.Serializable]
    public class EnemyData
    {
        public Enemy enemy;
        public int amount;
    }

    public List<EnemyData> enemies = new List<EnemyData>();
    public string levelName = "Fresh Start";
    public int hexCurrency = 5;
    public int goldCurrency = 100;
    public int lifeCurrency = 0;
    public int upgrades = 1;

}
