using System.Collections;
using System.Collections.Generic;
using UnityEngine;

   public static class AnimationCoroutine 
    {
        public static IEnumerator SetPositionVec2Coroutine(RectTransform rectTransform ,HexCell hexCell, Vector2 offset, AnimationCurve curve)
        {
            float time = 0;
            float _duration = 0.2f;

            while (time < _duration)
            {
                Vector2 position = Camera.main.WorldToScreenPoint(hexCell.transform.position);
                position -= new Vector2(Screen.width / 2, Screen.height / 2); // Adjust the screen position

                float y = curve.Evaluate(time / _duration);
                float x = curve.Evaluate(time / _duration);

                rectTransform.anchoredPosition = position + new Vector2(x * offset.x, y * offset.y);

                time += Time.deltaTime;
                yield return null;
            }

            Vector2 finalPosition = Camera.main.WorldToScreenPoint(hexCell.transform.position);
            finalPosition -= new Vector2(Screen.width / 2, Screen.height / 2); // Adjust the final screen position
            rectTransform.anchoredPosition = finalPosition + new Vector2(offset.x, offset.y);
        }

    }


