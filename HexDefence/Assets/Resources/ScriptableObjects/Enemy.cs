using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Enemy", order = 7)]
public class Enemy : ScriptableObject
{
	public GameObject prefab;
	public PooledObject deathEffect;
	public Sprite sprite;
	public string enemyName;
	public float health;
	public float armor;

	[Range(0, 100)]
	public int damage;

	[Range(0, 10)]
	public float speed;

	[Range(0, 1)]
	public float slow;

	[Range(0, 1000)]
	public int goldDrop;

	// public movmentType movmentType;
	public EnemyAnimation enemyDeathAnimation;
	public Enemy spawnOnDeath;
}

public enum movmentType
{
	normal,
	spurts,
	jumping,
	flying
}
