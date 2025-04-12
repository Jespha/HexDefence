using UnityEngine;

public class SpiralMovementPattern : IProjectileMovementPattern
{
	private float radius = 0.5f;
	private float rotationSpeed = 15f;
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
		// Calculate direct path vector
		Vector3 directVector = endPosition - currentPosition;
		float distanceToTarget = directVector.magnitude;

		// Calculate base forward movement
		Vector3 forward = directVector.normalized * speed * deltaTime;

		// Calculate axis perpendicular to movement direction
		Vector3 up = Vector3.up;
		Vector3 right = Vector3.Cross(directVector, up).normalized;
		if (right.magnitude < 0.001f)
		{
			// If movement is vertical, use another axis
			right = Vector3.Cross(directVector, Vector3.forward).normalized;
		}

		// Calculate spiral movement (circular motion around the direct path)
		float angle = Time.time * rotationSpeed;
		Vector3 offset =
			(
				right * Mathf.Cos(angle)
				+ Vector3.Cross(directVector.normalized, right) * Mathf.Sin(angle)
			) * radius;

		// Reduce spiral radius as we get closer to target
		float spiralFactor = Mathf.Min(1.0f, distanceToTarget / 5f);
		offset *= spiralFactor;

		// Combine direct movement with spiral offset
		return currentPosition + forward + offset;
	}

	public bool ShouldLookAtTarget()
	{
		return false;
	}

	public Quaternion GetRotation(
		Vector3 currentPosition,
		Vector3 startPosition,
		Vector3 endPosition,
		Vector3 moveDirection
	)
	{
		// Orient in direction of movement
		if (moveDirection.magnitude > 0.001f)
		{
			return Quaternion.LookRotation(moveDirection);
		}
		return Quaternion.LookRotation(endPosition - currentPosition);
	}
}
