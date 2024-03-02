using System;
using System.Collections.Generic;
using UnityEngine;

public class Currency : MonoBehaviour
{
    // Currency
    public static Currency Instance;
    public int HexCurrency {  get; private set; }
    public int GoldCurrency {  get; private set; }
    public int LifeCurrency {  get; private set; }
    public int MaxLifeCurrency {  get; private set; }

    // Locally linked objects
    [SerializeField] private TMPro.TextMeshProUGUI _hexCurrencyText;
    [SerializeField] private TMPro.TextMeshProUGUI _goldCurrencyText;
    [SerializeField] private TMPro.TextMeshProUGUI _lifeCurrencyText;
    [SerializeField] private Animator _hexCurrencyAnimator;
    [SerializeField] private Animator _goldCurrencyAnimator;
    [SerializeField] private Animator _lifeCurrencyAnimator;

    private void Update()
    {
        int _levelIndex = GameManager.Instance.Levels.LevelList.IndexOf(GameManager.Instance.CurrentLevel);
        //TODO: IMPLIMENT GAME OVER
        // if (LifeCurrency <= 0 && _levelIndex > 0) 
        // {
        //     GameManager.Instance.GameOver();
        // }
    }

    private void Awake() => Instance = this;

        ///<summary>Add currency to the player's inventory</summary>
        /// <param name="amount"></param>
        /// <param name="CurrencyType"></param>    

    public void UpdateCurrency(int amount, CurrencyType currencyType)
    {
        bool _positive = true;
        if (amount <= 0)
        {
            if (amount == 0)
            return;
            _positive = false;
        }
        switch (currencyType)
        {
            case CurrencyType.HexCurrency:
                HexCurrency += amount;
                if (_positive == true)
                _hexCurrencyAnimator.SetTrigger("AddCurrencyChange");
                else
                _hexCurrencyAnimator.SetTrigger("RemoveCurrencyChange");
                _hexCurrencyText.text = HexCurrency.ToString();
                break;
            case CurrencyType.GoldCurrency:
                GoldCurrency += amount;
                if (_positive == true)
                _goldCurrencyAnimator.SetTrigger("AddCurrencyChange");
                else
                _goldCurrencyAnimator.SetTrigger("RemoveCurrencyChange");
                _goldCurrencyText.text = GoldCurrency.ToString();
                break;
            case CurrencyType.LifeCurrency:
                LifeCurrency += amount;
                if (_positive == true)
                _lifeCurrencyAnimator.SetTrigger("AddCurrencyChange");
                else
                _lifeCurrencyAnimator.SetTrigger("RemoveCurrencyChange");
                _lifeCurrencyText.text = LifeCurrency.ToString() + "/" + MaxLifeCurrency.ToString();
                break;
            case CurrencyType.MaxLifeCurrency:
                MaxLifeCurrency += amount;
                if (_positive == true)
                _lifeCurrencyAnimator.SetTrigger("AddCurrencyChange");
                else
                _lifeCurrencyAnimator.SetTrigger("RemoveCurrencyChange");
                
                break;
        }
    }

    public void Notify(string message)
    {

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
