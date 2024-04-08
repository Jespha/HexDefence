using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class AnimationCoroutine
{
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

    public static IEnumerator SetPositionVec3Coroutine(
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
            Debug.Log("SetPositionVec3Coroutine");
            float t = curve.Evaluate(_time / _duration);
            rectTransform.position = Vector2LerpUnClamped(_startPosition, targetPosition, t);
            _time += Time.deltaTime;
            yield return null;
        }
    }

    public static Vector3 WorldToUISpace(Canvas parentCanvas, Vector3 worldPos)
    {
        
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out Vector2 movePos);
        return parentCanvas.transform.TransformPoint(movePos);
    }

    public static Vector2 Vector2LerpUnClamped( Vector2 a, Vector2 b, float t ){
        return t*b + (1-t)*a;
    }

    public static float3 FloatLerpUnClamped( float3 a, float3 b, float t ){
        return t*b + (1-t)*a;
    }

    public static Rect RectTransformToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        Vector2 position = (Vector2)transform.position;
        position.y += size.y * 0.5f; // Adjust the y position to account for the height
        position.x -= size.x * 0.5f; // Adjust the x position to account for the width
        return new Rect(position, size);
    }

}
