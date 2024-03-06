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
    [SerializeField] private AnimationCurve _currencyScaleGainCurve;
    [SerializeField] private PooledObject _currency;
    [SerializeField] private List<PooledObject> _currencySprites = new List<PooledObject>();
    private int _finalCurrencyAmount; // TODO: Replace with the actual type of the final currency amount

    private IEnumerator GainCurrencyAnimation(int amount, CurrencyType currencyType, Vector3 startPosition = new Vector3())
    {
        yield return new WaitUntil(() => PooledObjectManager.Instance != null);
        Vector2 _startPos = new Vector2(0, -50);
        Vector2 _endPos = new Vector2(0, 0);

        if (amount == 0)
            yield return null;
        
        for (int i = 0; i < amount; i++)
        {
            // Increase the duration as the index increases
            float _duration = 0.2f + (i * 0.001f);

            float _time = 0;
            Vector3 _endRoation = Vector3.forward * (-45 + i);
            PooledObject newCurrencySprite = PooledObjectManager.Instance.Get(_currency);
            newCurrencySprite.transform.SetParent(_this);
            newCurrencySprite.GetComponent<RectTransform>().anchoredPosition = _startPos;
            newCurrencySprite.gameObject.SetActive(false); // Set the newCurrencySprite to inactive
            while (_time < _duration)
            {
                newCurrencySprite.gameObject.SetActive(true); // Set the newCurrencySprite to active just before the animation starts
                _time += Time.deltaTime;
                float _t = _time / _duration;
                RectTransform _rect = newCurrencySprite.GetComponent<RectTransform>();
                newCurrencySprite.GetComponent<Image>().color = new Color(1, 1, 1, 1 - (_t*0.75f));
                _rect.anchoredPosition = Vector2.Lerp(_startPos, _endPos, _currencyGainCurve.Evaluate(_t));
                if(currencyType == CurrencyType.LifeCurrency)
                _rect.localScale = Vector3.Lerp(Vector3.one*1.5f,Vector3.one * 0.5f, _currencyScaleGainCurve.Evaluate(_t));
                else
                _rect.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.5f, _currencyScaleGainCurve.Evaluate(_t));
                _rect.localEulerAngles = Vector3.Lerp(_endRoation , Vector3.zero, _currencyGainCurve.Evaluate(_t));
                yield return null;
            }
            newCurrencySprite.gameObject.SetActive(false);
            _currencyAnimator.SetTrigger("AddCurrencyChange");
        }

        yield return null; 
    }

    public void UpdateCurrency(int amount, String CurrencyText, Vector3 currencyPosition = new Vector3(), CurrencyType currencyType = CurrencyType.HexCurrency)
    {
        if (amount > 0)
        {
            _currencyText.text = CurrencyText;
            if (currencyType == CurrencyType.GoldCurrency)
            amount = Mathf.RoundToInt(amount * .1f);
            if (currencyType != CurrencyType.MaxLifeCurrency)
            StartCoroutine(GainCurrencyAnimation(amount, currencyType, currencyPosition));
            else
            {
                _currencyText.text = CurrencyText;
                _currencyAnimator.SetTrigger("AddCurrencyChange");
            }
        }
        else
        {
            _currencyText.text = CurrencyText;
            _currencyAnimator.SetTrigger("RemoveCurrencyChange");
        }

    }

}
