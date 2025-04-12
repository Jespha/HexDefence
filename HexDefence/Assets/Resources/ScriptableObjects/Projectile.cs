using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "ScriptableObjects/Projectile", order = 7)]
public class Projectile : ScriptableObject
{
	public string Name;
	public PooledObject ProjectilePrefab;
	public PooledObject LaunchVFXPrefab;
	public PooledObject ImpactVFXPrefab;
}
