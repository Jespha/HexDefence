using System.Collections;
using System.Collections.Generic;
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
        float _duration
    )
    {
        float _time = 0;
        Vector2 startPosition = rectTransform.anchoredPosition;

        while (_time < _duration)
        {
            float t = curve.Evaluate(_time / _duration);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            _time += Time.deltaTime;
            yield return null;
        }
    }

    public static IEnumerator FadeCanvasGroup(
        float _duration,
        CanvasGroup canvasGroup,
        float targetAlpha,
        float waitTime = 0
    )
    {
        float _time = 0;
        if (targetAlpha <= 0)
        canvasGroup.blocksRaycasts = false;
        else
        canvasGroup.blocksRaycasts = true;
        
        yield return new WaitForSeconds(waitTime);
        while (_time < _duration)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, _time / _duration);
            _time += Time.deltaTime;
            yield return null;
        }

    }
}
