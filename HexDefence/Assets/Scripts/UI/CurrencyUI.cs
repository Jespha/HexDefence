using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyUI : MonoBehaviour
{
	[SerializeField]
	public RectTransform LocalRect;

	[SerializeField]
	private TMPro.TextMeshProUGUI _currencyText;

	[SerializeField]
	private Animator _currencyAnimator;

	[SerializeField]
	private AnimationCurve _currencyGainCurve;

	[SerializeField]
	private AnimationCurve _currencyScaleGainCurve;

	[SerializeField]
	private AnimationCurve _currencyRotationGainCurve;

	[SerializeField]
	private PooledObject _currency;

	[SerializeField]
	private List<PooledObject> _currencySprites = new List<PooledObject>();
	public int CurrentCurrencyAmount;
	List<Coroutine> _updateCurrencyCoroutines = new List<Coroutine>();

	private IEnumerator GainCurrencyAnimation(
		int amount,
		int perAmount,
		CurrencyType currencyType,
		Vector2 startPosition,
		PooledObject newCurrencySprite,
		int i
	)
	{
		yield return new WaitUntil(() => PooledObjectManager.Instance != null);
		// Vector2 _startPos = AnimationCoroutine.RectTransformToScreenSpace(startPosition).position;
		Vector2 _startPos = new Vector2(startPosition.x, -startPosition.y);

		float _duration = 1f;
		float _time = 0;
		Vector3 _endRotation = Vector3.forward * 45;
		Vector2 localStartPosition = LocalRect.InverseTransformPoint(startPosition);
		Vector2 _endPos = LocalRect.anchoredPosition;

		// Generate random initial direction and magnitude
		float randomAngle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
		float randomMagnitude = UnityEngine.Random.Range(60f, 140f);
		Vector2 randomDirection = new Vector2(
			Mathf.Cos(randomAngle) * randomMagnitude,
			Mathf.Sin(randomAngle) * randomMagnitude
		);

		// Create a waypoint with random offset
		Vector2 waypoint = localStartPosition + randomDirection;
		float waypointTime = UnityEngine.Random.Range(0.2f, 0.4f);

		newCurrencySprite.transform.SetParent(LocalRect);
		newCurrencySprite.gameObject.SetActive(false);

		while (_time < _duration)
		{
			newCurrencySprite.gameObject.SetActive(true);
			_time += Time.deltaTime;
			float _t = _time / _duration;
			RectTransform _rect = newCurrencySprite.GetComponent<RectTransform>();
			newCurrencySprite.GetComponent<Image>().color = new Color(1, 1, 1, 1.5f - (_t * 0.1f));

			// Use the waypoint for more interesting motion
			Vector2 currentPos;
			if (_t < waypointTime)
			{
				// First move toward random waypoint
				currentPos = AnimationCoroutine.Vector2LerpUnClamped(
					localStartPosition,
					waypoint,
					_t / waypointTime
				);
			}
			else
			{
				// Then curve toward the destination
				float curveT = (_t - waypointTime) / (1 - waypointTime);
				currentPos = AnimationCoroutine.Vector2LerpUnClamped(
					waypoint,
					_endPos,
					_currencyGainCurve.Evaluate(curveT)
				);
			}

			_rect.anchoredPosition = currentPos;
			_rect.localScale = AnimationCoroutine.Vector3LerpUnClamped(
				Vector3.one,
				Vector3.one * 1.5f,
				_currencyScaleGainCurve.Evaluate(_t)
			);
			_rect.localEulerAngles = AnimationCoroutine.Vector3LerpUnClamped(
				Vector3.forward,
				_endRotation,
				_currencyRotationGainCurve.Evaluate(_t)
			);
			yield return null;
		}
		newCurrencySprite.gameObject.SetActive(false);
		_currencyAnimator.SetTrigger("AddCurrencyChange");

		if (currencyType != CurrencyType.MaxLifeCurrency)
		{
			if (currencyType == CurrencyType.LifeCurrency)
			{
				CurrentCurrencyAmount += perAmount;
				_currencyText.text =
					CurrentCurrencyAmount.ToString()
					+ "/"
					+ Currency.Instance.MaxLifeCurrency.ToString();
			}
			else
			{
				CurrentCurrencyAmount += perAmount;
				_currencyText.text = CurrentCurrencyAmount.ToString();
			}
		}
		yield return null;
	}

	public void SetTempCurrencyText(int amount, CurrencyType currencyType)
	{
		_currencyAnimator.SetTrigger("Show");
		CurrentCurrencyAmount = amount;
		_currencyText.text = amount.ToString();
	}

	private IEnumerator NullCurrencyAnimator(CurrencyType currencyType)
	{
		int initialCurrencyAmount;
		int removeAmount = 1;
		if (currencyType == CurrencyType.GoldCurrency)
		{
			initialCurrencyAmount = Mathf.RoundToInt(CurrentCurrencyAmount / 10.0f);
			removeAmount = 10;
		}
		else
			initialCurrencyAmount = CurrentCurrencyAmount;
		for (int i = 0; i < initialCurrencyAmount; i++)
		{
			float delay = 0.01f + 0.05f * ((initialCurrencyAmount - i) / (float)initialCurrencyAmount);
			_currencyText.text = CurrentCurrencyAmount.ToString();
			yield return new WaitForSeconds(delay);
			CurrentCurrencyAmount -= removeAmount;
		}
		_currencyText.text = "";
		_currencyAnimator.SetTrigger("Hide");
		yield return null;
	}

	public void NullCurrency(CurrencyType currencyType)
	{
		StartCoroutine(NullCurrencyAnimator(currencyType));
	}

	public IEnumerator UpdateCurrency(
		int amount,
		String CurrencyText,
		Vector3 currencyPosition,
		CurrencyType currencyType = CurrencyType.HexCurrency
	)
	{
		int _amount = 0;
		int _perAmount = 0;
		if (amount > 0)
		{
			if (currencyType == CurrencyType.GoldCurrency)
			{
				_perAmount = 10;
				_amount = Mathf.RoundToInt(amount / 10.0f);
			}

			if (
				currencyType == CurrencyType.HexCurrency
				|| currencyType == CurrencyType.LifeCurrency
			)
			{
				_amount = amount;
				_perAmount = 1;
			}

			if (currencyType != CurrencyType.MaxLifeCurrency)
			{
				yield return new WaitForSeconds(1.2f);
				for (int i = 0; i < _amount; i++)
				{
					PooledObject _newCurrencySprite = PooledObjectManager.Instance.Get(_currency);
					StartAndTrackCoroutine(
						GainCurrencyAnimation(
							_amount,
							_perAmount,
							currencyType,
							currencyPosition,
							_newCurrencySprite,
							i
						)
					);

					float delay = 10 * (i / amount);
					yield return new WaitForSeconds(delay);
				}
			}
			else
			{
				_currencyAnimator.SetTrigger("AddCurrencyChange");
			}
		}
		else
		{
			CurrentCurrencyAmount += amount; 
			_currencyText.text = CurrencyText;
			_currencyAnimator.SetTrigger("RemoveCurrencyChange");
		}
	}

	public void StartAndTrackCoroutine(IEnumerator coroutine)
	{
		Coroutine newCoroutine = StartCoroutine(coroutine);
		_updateCurrencyCoroutines.Add(newCoroutine);
		StartCoroutine(TrackCoroutine(coroutine, newCoroutine));
	}

	private IEnumerator TrackCoroutine(IEnumerator coroutine, Coroutine newCoroutine)
	{
		yield return newCoroutine;
		_updateCurrencyCoroutines.Remove(newCoroutine);
	}
}
