using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Currency : MonoBehaviour
{
    // Currency
    public static Currency Instance;
    [SerializeField] private int _hexCurrency;
    [SerializeField] private int _GoldCurrency;
    public int HexCurrency { get => _hexCurrency; set => _hexCurrency = value; }
    public int GoldCurrency { get => _GoldCurrency; set => _GoldCurrency = value; }

    // Locally linked objects
    [SerializeField] private TMPro.TextMeshProUGUI _hexCurrencyText;
    [SerializeField] private TMPro.TextMeshProUGUI _goldCurrencyText;
    [SerializeField] private Animator _hexCurrencyAnimator;
    [SerializeField] private Animator _goldCurrencyAnimator;

    private void Update()
    {
        _hexCurrencyText.text = _hexCurrency.ToString();
        _goldCurrencyText.text = _GoldCurrency.ToString();
    }

    private void Awake() => Instance = this;

    public void AddCurrency(int amount)
    {
        _hexCurrency += amount;
        _hexCurrencyAnimator.SetTrigger("AddCurrencyChange");
    }

    public void RemoveCurrency(int amount)
    {
        _hexCurrency -= amount;
        _hexCurrencyAnimator.SetTrigger("RemoveCurrencyChange");
    }
}
