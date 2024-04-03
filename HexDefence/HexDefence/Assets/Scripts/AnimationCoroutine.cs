using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class AnimationCoroutine
{
    public static IEnumerator SetPositionVec2Coroutine(
        RectTransform rectTransform,
        HexCell hexCell,
        Vector2 offset,
        AnimationCurve curve
    )
    {
        float _time = 0;
        float _duration = 0.2f;

        while (_time < _duration)
        {
            Vector2 position = Camera.main.WorldToScreenPoint(hexCell.transform.position);
            position -= new Vector2(Screen.width / 2, Screen.height / 2); // Adjust the screen position

            float y = curve.Evaluate(_time / _duration);
            float x = curve.Evaluate(_time / _duration);

            rectTransform.anchoredPosition = position + new Vector2(x * offset.x, y * offset.y);
            _time += Time.deltaTime;
            yield return null;
        }

        Vector2 finalPosition = Camera.main.WorldToScreenPoint(hexCell.transform.position);
        finalPosition -= new Vector2(Screen.width / 2, Screen.height / 2); // Adjust the final screen position
        rectTransform.anchoredPosition = finalPosition + new Vector2(offset.x, offset.y);
    }

    public static IEnumerator SetPositionVec2Coroutine(
        RectTransform rectTransform,
        Vector2 targetPosition,
        AnimationCurve curve,
        float _duration,
        float _waitTime = 0
    )
    {
        float _time = 0;
        Vector2 _startPosition = rectTransform.anchoredPosition;
        yield return new WaitForSeconds(_waitTime);

        while (_time < _duration)
        {
            float t = curve.Evaluate(_time / _duration);
            rectTransform.anchoredPosition = Vector2LerpUnClamped(_startPosition, targetPosition, t);
            _time += Time.deltaTime;
            yield return null;
        }
    }

    public static IEnumerator FadeCanvasGroup(
        float _duration,
        CanvasGroup canvasGroup,
        float targetAlpha,
        float _waitTime = 0
    )
    {
        float _time = 0;
        if (targetAlpha <= 0)
        canvasGroup.blocksRaycasts = false;
        else
        canvasGroup.blocksRaycasts = true;
        
        yield return new WaitForSeconds(_waitTime);
        while (_time < _duration)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, _time / _duration);
            _time += Time.deltaTime;
            yield return null;
        }

    }

    public static IEnumerator SetScaleVec2Coroutine(
        RectTransform rectTransform,
        Vector2 _startScale,
        Vector2 targetScale,
        AnimationCurve curve,
        float _duration,
        float _waitTime = 0
    )
    {
        float _time = 0;
        yield return new WaitForSeconds(_waitTime);

        while (_time < _duration)
        {
            float t = curve.Evaluate(_time / _duration);
            rectTransform.localScale = Vector2LerpUnClamped(_startScale, targetScale, t);
            _time += Time.deltaTime;
            yield return null;
        }

        rectTransform.localScale = targetScale;
    }

    public static IEnumerator IdleFloatRotate(RectTransform rectTransform, float speed, float maxRotation, AnimationCurve curve)
    {
        float time = 0;
        AnimationCurve _curve = new AnimationCurve();
        _curve.AddKey(0, 0);
        _curve.AddKey(0.5f, 1);
        _curve.AddKey(1, 0);
        _curve.keys[0].outTangent = 0;
        _curve.keys[1].inTangent = 1;
        _curve.keys[1].outTangent = -1;
        _curve.keys[2].inTangent = 0;

        while (true)
        {
            float t = curve.Evaluate(Mathf.PingPong(time * speed, 1));
            float rotation = maxRotation * t - maxRotation / 2;

            rectTransform.localRotation = Quaternion.Euler(rotation, rotation, rotation);

            time += Time.deltaTime;
            yield return null;
        }

    }

    public static Vector2 Vector2LerpUnClamped( Vector2 a, Vector2 b, float t ){
        return t*b + (1-t)*a;
    }

    public static float3 FloatLerpUnClamped( float3 a, float3 b, float t ){
        return t*b + (1-t)*a;
    }

}
