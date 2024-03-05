using System.Collections;
using System.Collections.Generic;
using BBX.Dialogue.GUI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField]
    private TextMeshProUGUI _levelText;

    [SerializeField]
    private TextMeshProUGUI _levelTitleText;

    [SerializeField]
    private RectTransform AnimationParent;

    [Header("Animation")]
    [SerializeField]
    private AnimationCurve _curve;

    [SerializeField]
    private Vector2 _updateOffset;

    [SerializeField]
    private CanvasGroup _gameStateCanvasGroup;

    [SerializeField]
    private CanvasGroup _nextLevelButtonCanvasGroup;

    [SerializeField]
    private List<Image> _gameStateImage;

    private void Start()
    {
        _nextLevelButtonCanvasGroup.alpha = 0;
        _nextLevelButtonCanvasGroup.blocksRaycasts = false;
    }

    public void UpdateLevel(int level)
    {
        StartCoroutine(
            AnimationCoroutine.SetPositionVec2Coroutine(this.AnimationParent, _updateOffset, _curve, 0.5f)
        );
        _levelText.text = "Level: " + level;
        _levelText.GetComponent<TextMeshAnimator>().RunText();
        if (GameManager.Instance.Levels.LevelList.IndexOf(GameManager.Instance.CurrentLevel) != 0)
        StartCoroutine(AnimationCoroutine.FadeCanvasGroup(1, _nextLevelButtonCanvasGroup, 0));
        if (GameManager.Instance.CurrentLevel != null)
            _levelTitleText.text = GameManager.Instance.CurrentLevel.levelName;
        else
            _levelTitleText.text = " ";
        _levelTitleText.color = new Color(1, 1, 1, 1);
        UpdateGamePhaseUI(GameManager.Instance.GamePhase);
    }

    public void UpdateLevel(int level, bool complete = false)
    {
        if (complete)
        {
            StartCoroutine(AnimationCoroutine.FadeCanvasGroup(1, _gameStateCanvasGroup, 1));
            _levelText.text = "Level: " + level + "<br> COMPLETE!";
            _levelText.ForceMeshUpdate();
            _levelText.GetComponent<TextMeshAnimator>().RunText();
        }
        else
        {
            UpdateLevel(level);
        }
    }

    public void UpdateGamePhaseUI(GamePhase gamePhase)
    {
        switch (gamePhase)
        {
            case GamePhase.Income:
                StartCoroutine(IncomePhase());
                break;
            case GamePhase.HexPlacement:
                StartCoroutine(FillGameStateImage(_gameStateImage[1], 1));
                _levelText.text = "Hex Placment Phase";
                break;
            case GamePhase.Build:
                StartCoroutine(FillGameStateImage(_gameStateImage[2], 1));
                _levelText.text = "Build Phase";
                StartCoroutine(
                    AnimationCoroutine.FadeCanvasGroup(1, _nextLevelButtonCanvasGroup, 1, 1)
                );
                break;
        }
    }

    private IEnumerator IncomePhase()
    {
        StartCoroutine(FillGameStateImage(_gameStateImage[0], 1));
        _levelTitleText.color = new Color(1, 1, 1, 0);
        _levelText.text = "Income Phase";
        AnimationCoroutine.FadeCanvasGroup(1, _gameStateCanvasGroup, 1, 0);
        yield return new WaitForSeconds(3);
        GameManager.Instance.SetGamePhase(GamePhase.HexPlacement);
    }

    private IEnumerator FillGameStateImage(Image image, float fillAmount)
    {
        float time = 0;
        float _duration = 0.5f;
        while (time < _duration)
        {
            image.fillAmount = Mathf.Lerp(image.fillAmount, fillAmount, time / _duration);
            time += Time.deltaTime;
            yield return null;
        }
    }

    public void StartGameIfPossible()
    {   
    if (GameManager.Instance.GamePhase == GamePhase.HexPlacement || GameManager.Instance.GamePhase == GamePhase.Build)
        {
            GameManager.Instance.LoadLevelIfPossible();
            StartCoroutine(AnimationCoroutine.FadeCanvasGroup(1, _nextLevelButtonCanvasGroup, 0));
            StartCoroutine(AnimationCoroutine.FadeCanvasGroup(0.5f, _gameStateCanvasGroup, 0));
        }
        else
        {
            Debug.Log("Cannot start game"); // TODO: Add a UI element to show this
        }
    }
}
