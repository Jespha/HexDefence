using UnityEngine;

public interface IProjectileMovementPattern
{
	Vector3 CalculateNextPosition(
		Vector3 currentPosition,
		Vector3 startPosition,
		Vector3 endPosition,
		float progress,
		float speed,
		float deltaTime
	);

	bool ShouldLookAtTarget();

	Quaternion GetRotation(
		Vector3 currentPosition,
		Vector3 startPosition,
		Vector3 endPosition,
		Vector3 moveDirection
	);

	bool IsHoming { get; }
}
