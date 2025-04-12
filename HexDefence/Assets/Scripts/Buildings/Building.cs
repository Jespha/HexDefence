using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
	public HexCell _hexCell { get; private set; }

	[SerializeField]
	private AnimationCurve _clickCurve;
	private float _duration = 0.4f;

	public void Initialize(HexCell hexCell)
	{
		_hexCell = hexCell;
	}

	private void Start()
	{
		AnimateScaleCoroutine(this.transform);
	}

	private IEnumerator AnimateScaleCoroutine(Transform _transform)
	{
		float time = 0;

		while (time < _duration)
		{
			float scale = _clickCurve.Evaluate(time / _duration);
			_transform.localScale = new Vector3(scale, scale, scale);

			time += Time.deltaTime;
			yield return null;
		}

		// ensure the final scale is set correctly
		float finalScale = _clickCurve.Evaluate(1);
		_transform.localScale = new Vector3(finalScale, finalScale, finalScale);
	}
}
