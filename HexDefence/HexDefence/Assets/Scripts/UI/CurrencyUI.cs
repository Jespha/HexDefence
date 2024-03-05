using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private RectTransform _this;
    [SerializeField] private TMPro.TextMeshProUGUI _currencyText;
    [SerializeField] private Animator _currencyAnimator;
    [SerializeField] private AnimationCurve _currencyGainCurve;
    [SerializeField] private GameObject _currency;
    [SerializeField] private List<GameObject> _currencySprites = new List<GameObject>();

    private IEnumerator GainCurrencyAnimation(int amount)
    {

        float _duration = 0.1f;
        // initalize a list of images moving from _startPos to _endPos along the _currencyGainCurve
        Vector2 _startPos = new Vector2(0, -50);
        Vector2 _endPos = new Vector2(0, 0);

        if (amount == 0)
        yield return null;
        
        for (int i = 0; i < amount; i++)
        {
            float _time = 0;
            GameObject newCurrencySprite = Instantiate(_currency, _this);
            newCurrencySprite.GetComponent<RectTransform>().anchoredPosition = _startPos;
            _currencySprites.Add(newCurrencySprite);
            while (_time < _duration)
            {
                _time += Time.deltaTime;
                float _t = _time / _duration;
                RectTransform _rect = _currencySprites[i].GetComponent<RectTransform>();
                _currencySprites[i].GetComponent<Image>().color = new Color(1, 1, 1, 1 - _t);
                _rect.anchoredPosition = Vector2.Lerp(_startPos, _endPos, _currencyGainCurve.Evaluate(_t));
                // if (_rect.anchoredPosition == _endPos)
                // {
                //     DestroyImmediate(_currencySprites[i]);
                //     _currencySprites.RemoveAt(i);
                //     yield return null;
                // }
                yield return null;
            }
        }

        yield return null; 
    }

    public void UpdateCurrency(int amount, String CurrencyText, Vector3 currencyPosition = new Vector3(), CurrencyType currencyType = CurrencyType.HexCurrency)
    {
        if (amount > 0)
        {
            _currencyText.text = CurrencyText;
            _currencyAnimator.SetTrigger("AddCurrencyChange");
            _currencySprites.Clear();
            if (currencyType == CurrencyType.GoldCurrency)
            amount = Mathf.RoundToInt(amount * .1f);
                StartCoroutine(GainCurrencyAnimation(amount));
            StartCoroutine(GainCurrencyAnimation(amount));
        }
        else
        {
            _currencyText.text = CurrencyText;
            _currencyAnimator.SetTrigger("RemoveCurrencyChange");
        }

    }

}
