using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
	fileName = "EnemyAnimation",
	menuName = "ScriptableObjects/EnemyAnimation",
	order = 5
)]
public class EnemyAnimation : ScriptableObject
{
	public String Name;
	public AnimationCurve _animationCurve;
	public float _duration;
	public float _scale;
	public float _speed;
	public bool _x,
		_y,
		_z;
}

public enum DeathAnimationType
{
	fall,
	explode,
	burn,
	melt,
	vaporize
}
