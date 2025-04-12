using System;
using System.Collections.Generic;
using UnityEngine;

public class Currency : MonoBehaviour
{
	// Currency
	public static Currency Instance;
	public int HexCurrency { get; private set; }
	public int GoldCurrency { get; private set; }
	public int LifeCurrency { get; private set; }
	public int MaxLifeCurrency { get; private set; }

	public Dictionary<CurrencyType, int> CurrencyDictionary { get; private set; }

	// Locally linked objects
	[SerializeField]
	private List<CurrencyUI> _currencyUI = new List<CurrencyUI>();

	[SerializeField]
	private float _lastGoldGain;

	[SerializeField]
	private float _lastHexGain;

	[SerializeField]
	private float _lastLifeGain;

	[SerializeField]
	private float _lastMaxLifeGain;

	private void Start()
	{
		if (_currencyUI.Count <= 1)
		{
			Debug.Log("CurrencyUI not found in Currency");
		}
	}

	private void Update()
	{
		// int _levelIndex = GameManager.Instance.Levels.LevelList.IndexOf(
		//     GameManager.Instance.CurrentLevel
		// );
		//TODO: IMPLIMENT GAME OVER
		// if (LifeCurrency <= 0 && _levelIndex > 0)
		// {
		//     GameManager.Instance.GameOver();
		// }
	}

	private void Awake()
	{
		Instance = this;

		CurrencyDictionary = new Dictionary<CurrencyType, int>();

		foreach (CurrencyType type in Enum.GetValues(typeof(CurrencyType)))
		{
			var property = GetType().GetProperty(type.ToString());
			if (property != null)
			{
				CurrencyDictionary[type] = (int)property.GetValue(this);
			}
		}
	}

	///<summary>Add currency to the player's inventory</summary>
	/// <param name="amount"></param>
	/// <param name="CurrencyType"></param>
	public void UpdateCurrency(int amount, CurrencyType currencyType, Vector3 currencyPosition)
	{
		if (currencyPosition == null)
		{
			currencyPosition = new Vector3();
		}
		switch (currencyType)
		{
			case CurrencyType.LifeCurrency:
				LifeCurrency += amount;
				StartCoroutine(
					_currencyUI[0]
						.UpdateCurrency(
							amount,
							LifeCurrency.ToString() + "/" + MaxLifeCurrency.ToString(),
							currencyPosition,
							CurrencyType.LifeCurrency
						)
				);
				break;
			case CurrencyType.MaxLifeCurrency:
				MaxLifeCurrency += amount;
				StartCoroutine(
					_currencyUI[0]
						.UpdateCurrency(
							amount,
							LifeCurrency.ToString() + "/" + MaxLifeCurrency.ToString(),
							currencyPosition,
							CurrencyType.MaxLifeCurrency
						)
				);
				break;
			case CurrencyType.HexCurrency:
				HexCurrency += amount;
				StartCoroutine(
					_currencyUI[1]
						.UpdateCurrency(
							amount,
							HexCurrency.ToString(),
							currencyPosition,
							CurrencyType.HexCurrency
						)
				);
				break;
			case CurrencyType.GoldCurrency:
				GoldCurrency += amount;
				StartCoroutine(
					_currencyUI[2]
						.UpdateCurrency(
							amount,
							GoldCurrency.ToString(),
							currencyPosition,
							CurrencyType.GoldCurrency
						)
				);
				break;
		}
	}
}

/// <summary>
/// The type of currency (HexCurrency, GoldCurrency, LifeCurrency)
/// </summary>
public enum CurrencyType
{
	HexCurrency,
	GoldCurrency,
	LifeCurrency,
	MaxLifeCurrency
}
