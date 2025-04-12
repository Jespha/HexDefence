using UnityEngine;

public class DirectMovementPattern : IProjectileMovementPattern
{
	public bool IsHoming => true;

	public Vector3 CalculateNextPosition(
		Vector3 currentPosition,
		Vector3 startPosition,
		Vector3 endPosition,
		float progress,
		float speed,
		float deltaTime
	)
	{
		// Simple linear movement toward the target
		return Vector3.MoveTowards(currentPosition, endPosition, speed * deltaTime);
	}

	public bool ShouldLookAtTarget()
	{
		return true; // Always look at the target for direct movement
	}

	public Quaternion GetRotation(
		Vector3 currentPosition,
		Vector3 startPosition,
		Vector3 endPosition,
		Vector3 moveDirection
	)
	{
		return Quaternion.LookRotation(moveDirection.normalized);
	}
}
