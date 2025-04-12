using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
	fileName = "HexBuildings",
	menuName = "ScriptableObjects/HexBuildings",
	order = 20
)]
public class HexBuildings : ScriptableObject
{
	[Header("Hex Buildings")]
	[SerializeField]
	private List<HexBuilding> hexBuildings = new List<HexBuilding>();
}
