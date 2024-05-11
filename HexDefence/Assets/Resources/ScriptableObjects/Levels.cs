using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LEVELS", menuName = "ScriptableObjects/Levels", order = 10)]
public class Levels : ScriptableObject
{

    public List<Level> LevelList = new List<Level>();

}
