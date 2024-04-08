using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private int _finalCurrencyAmount;
    private int _currentCurrencyAmount;

    private IEnumerator GainCurrencyAnimation(int amount, int perAmount, CurrencyType currencyType, Vector2 startPosition)
    {
        yield return new WaitUntil(() => PooledObjectManager.Instance != null);
        // Vector2 _startPos = AnimationCoroutine.RectTransformToScreenSpace(startPosition).position;
        Vector2 _startPos = new Vector2(startPosition.x, -startPosition.y);

        if (amount == 0)
            yield return null;
        
        for (int i = 0; i < amount; i++)
        {
            float _duration = Math.Min(0.2f, 0f + (0.4f * i / amount));
            float _time = 0;
            Vector3 _endRotation = Vector3.forward * (-45 + i);
            PooledObject newCurrencySprite = PooledObjectManager.Instance.Get(_currency);
            Vector2 localStartPosition = _this.InverseTransformPoint(startPosition);
            Vector2 _endPos = _this.anchoredPosition;
            newCurrencySprite.transform.SetParent(_this);
            newCurrencySprite.gameObject.SetActive(false); 
            while (_time < _duration)
            {
                newCurrencySprite.gameObject.SetActive(true);
                _time += Time.deltaTime;
                float _t = _time / _duration;
                RectTransform _rect = newCurrencySprite.GetComponent<RectTransform>();
                newCurrencySprite.GetComponent<Image>().color = new Color(1, 1, 1, 1.5f - (_t*0.1f));
                _rect.anchoredPosition = AnimationCoroutine.Vector2LerpUnClamped(localStartPosition, _endPos, _currencyGainCurve.Evaluate(_t));
                if(currencyType == CurrencyType.LifeCurrency)
                _rect.localScale = Vector3.Lerp(Vector3.one*1.5f,Vector3.one * 0.5f, _currencyScaleGainCurve.Evaluate(_t));
                else
                _rect.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.5f, _currencyScaleGainCurve.Evaluate(_t));
                _rect.localEulerAngles = Vector3.Lerp(Vector3.zero, _endRotation, _currencyGainCurve.Evaluate(_t));
                yield return null;
            }
            newCurrencySprite.gameObject.SetActive(false);
            _currencyAnimator.SetTrigger("AddCurrencyChange");

            if (currencyType != CurrencyType.MaxLifeCurrency)
            {   
                if (currencyType == CurrencyType.LifeCurrency)
                {
                    _currentCurrencyAmount += perAmount;
                    _currencyText.text = _currentCurrencyAmount.ToString() + "/" + Currency.Instance.MaxLifeCurrency.ToString();
                }
                else
                {
                    _currentCurrencyAmount += perAmount;
                    _currencyText.text = _currentCurrencyAmount.ToString();
                }
            }

            yield return null; 
        }
    }

    public void UpdateCurrency(int amount, String CurrencyText, Vector3 currencyPosition, CurrencyType currencyType = CurrencyType.HexCurrency)
    {
        _currentCurrencyAmount = Currency.Instance.CurrencyDictionary[currencyType];
        int _amount = 0;

        if (amount > 0)
        {
            if (currencyType == CurrencyType.GoldCurrency)
            {
                _amount = Mathf.RoundToInt(amount/10);
                amount = Mathf.RoundToInt(amount * .1f);
            }

            if (currencyType == CurrencyType.HexCurrency || currencyType == CurrencyType.LifeCurrency)
            {
                _amount = 1;
            }

            if (currencyType != CurrencyType.MaxLifeCurrency)
            StartCoroutine(GainCurrencyAnimation(amount,_amount, currencyType, currencyPosition));
            else
            {
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