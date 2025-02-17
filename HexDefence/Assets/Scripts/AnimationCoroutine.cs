using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class AnimationCoroutine
{
    public static IEnumerator SetAnchoredPositionVec2Coroutine(
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


    public static IEnumerator SetPositionVec2Coroutine(
        RectTransform rectTransform,
        Vector2 targetPosition,
        AnimationCurve curve,
        float _duration,
        float _waitTime = 0
    )
    {

        float _time = 0;
        Vector2 _startPosition = rectTransform.position;
        yield return new WaitForSeconds(_waitTime);

        while (_time < _duration)
        {
            float t = curve.Evaluate(_time / _duration);
            rectTransform.position = Vector2LerpUnClamped(_startPosition, targetPosition, t);
            _time += Time.deltaTime;
            yield return null;
        }
        rectTransform.position = targetPosition;
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
    
        canvasGroup.alpha = targetAlpha;
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

    public static IEnumerator SetScaleVec3Coroutine(
        Transform transform,
        Vector3 _startScale,
        Vector3 targetScale,
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
            transform.localScale = Vector3LerpUnClamped(_startScale, targetScale, t);
            _time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
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


    public static IEnumerator AnimatePositionCoroutine(Transform _transform, Vector3 offset, float _wait, float _duration, AnimationCurve _curve)
    {
        float time = 0;
        Vector3 originalPosition = _transform.localPosition;
        Vector3 targetPosition = originalPosition + offset;

        if (_wait != 0)
            yield return new WaitForSeconds(_wait);

        while (time < _duration)
        {
            Vector3 newPosition = Vector3LerpUnClamped(targetPosition, originalPosition, _curve.Evaluate(time / _duration));
            _transform.localPosition = newPosition;
            time += Time.deltaTime;
            yield return null;
        }
    }

    public static IEnumerator AnimatePositionCoroutine(Transform _transform, Vector3 offset, float _wait, float _duration, AnimationCurve _curve, MeshRenderer _meshRenderer = null)
    {
        float time = 0;
        Vector3 originalPosition = _transform.localPosition;
        Vector3 targetPosition = originalPosition + offset;

        if (_wait != 0)
        {
            _meshRenderer.enabled = false;
            yield return new WaitForSeconds(_wait);
        }

        _meshRenderer.enabled = true;

        while (time < _duration)
        {
            Vector3 newPosition = Vector3LerpUnClamped(targetPosition, originalPosition, _curve.Evaluate(time / _duration));
            _transform.localPosition = newPosition;
            time += Time.deltaTime;
            yield return null;
        }
    }
    
    public static IEnumerator AnimatedVector2ToZero(SelectedHexCell selectedHexCell , Vector2 _currentOffset, AnimationCurve _curve, float _duration, float _wait = 0)
    {
        Vector2 startValue = selectedHexCell.CurrentOffset;
        float duration = 1.0f; // duration of the animation in seconds
        float time = 0.0f;

        if (_wait != 0)
        {
            yield return new WaitForSeconds(_wait);
        }

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // Interpolate the current offset to zero
            selectedHexCell.CurrentOffset = Vector2LerpUnClamped(startValue, Vector2.zero, _curve.Evaluate(time / _duration));

            yield return null;
        }

        // Ensure the final value is exactly zero
        selectedHexCell.CurrentOffset = Vector2.zero;
    }

    public static Vector3 WorldToUISpace(Canvas parentCanvas, Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out Vector2 movePos);
        return parentCanvas.transform.TransformPoint(movePos);
    }

    public static Vector3 WorldToLocalUISpace(Canvas parentCanvas, Vector3 worldPos, RectTransform rectTransform)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPos, parentCanvas.worldCamera, out Vector2 movePos);
        return rectTransform.transform.TransformPoint(movePos);
    }

    public static Vector2 Vector2LerpUnClamped( Vector2 a, Vector2 b, float t ){
        return t*b + (1-t)*a;
    }

    public static Vector3 Vector3LerpUnClamped( Vector3 a, Vector3 b, float t ){
        return t*b + (1-t)*a;
    }

    public static float3 FloatLerpUnClamped( float3 a, float3 b, float t ){
        return t*b + (1-t)*a;
    }

    public static Rect RectTransformToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        Vector2 position = (Vector2)transform.position;
        position.y -= 0;
        position.x += 0;
        return new Rect(position, size);
    }

}
