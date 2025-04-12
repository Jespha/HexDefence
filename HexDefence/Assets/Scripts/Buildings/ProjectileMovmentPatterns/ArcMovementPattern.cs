using UnityEngine;

public class ArcMovementPattern : IProjectileMovementPattern
{
	private float arcHeight = 2.5f;
	public bool IsHoming => false;

	public Vector3 CalculateNextPosition(
		Vector3 currentPosition,
		Vector3 startPosition,
		Vector3 endPosition,
		float progress,
		float speed,
		float deltaTime
	)
	{
		// Calculate direct vector from start to target
		Vector3 startToEnd = endPosition - startPosition;

		// Use the provided progress parameter
		float normalizedProgress = progress;

		// Direct interpolation from start to end based on progress
		Vector3 directPath = Vector3.Lerp(startPosition, endPosition, normalizedProgress);

		// Calculate arc height using sin curve (peak in the middle)
		float arcOffset = Mathf.Sin(normalizedProgress * Mathf.PI) * arcHeight;

		// Apply arc height
		return directPath + new Vector3(0, arcOffset, 0);
	}

	public bool ShouldLookAtTarget()
	{
		return false; // Look in direction of movement for arcs
	}

	public Quaternion GetRotation(
		Vector3 currentPosition,
		Vector3 startPosition,
		Vector3 endPosition,
		Vector3 moveDirection
	)
	{
		// Look in movement direction if it's significant
		if (moveDirection.magnitude > 0.001f)
		{
			return Quaternion.LookRotation(moveDirection);
		}
		return Quaternion.LookRotation(endPosition - currentPosition);
	}
}
