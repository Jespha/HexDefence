using UnityEngine;

public class OnHitEffect : MonoBehaviour
{
	public static string Name;
	public OnHitEffectType OnHitEffectType { get; set; }

	public void OnHitEffectDamage(float damage, EnemyData enemyData)
	{
		enemyData.Health -= damage;
	}

	public void OnHitEffectSplash(float damage, EnemyData enemyData)
	{
		// instantiate splash effect damage enemies in range
	}
}

public enum OnHitEffectType
{
	Damage,
	Splash,
	// Area,
	// Economy,
	// Beam,
	// Buff,
	// None,
	// Damage,
	// Slow,
	// Stun,
	// Freeze,
	// Burn,
	// Poison,
	// Heal,
	// Debuff,
	// Summon,
	// Knockback,
	// Pull,
	// Push,
	// Teleport,
	// Disarm,
	// Silence,
	// Fear,
	// Charm,
	// Taunt,
	// Sleep,
	// Root,
	// Blind,
}
