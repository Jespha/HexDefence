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
    private RectTransform animationParent;

    [Header("Animation")]
    [SerializeField]
    private AnimationCurve curve;

    [SerializeField]
    private Vector2 updateOffset;

    [SerializeField]
    private CanvasGroup _gameStateCanvasGroup;

    [SerializeField]
    private CanvasGroup _nextLevelButtonCanvasGroup;

    [SerializeField]
    private List<Image> _gameStateImage;

    [Header("Level Complete Screen")]
    [SerializeField]
    private RectTransform levelCompleteAnimationParent;

    [SerializeField]
    private CanvasGroup _levelCompleteCanvasGroup;
    [SerializeField]
    private AnimationCurve _levelCompleteCurve;
    public List<CurrencyUI> LevelCompleteCurrencyAnimationParent;

    private void Start()
    {
        _nextLevelButtonCanvasGroup.alpha = 0;
        _nextLevelButtonCanvasGroup.blocksRaycasts = false;
        _levelCompleteCanvasGroup.alpha = 0;
        _levelCompleteCanvasGroup.blocksRaycasts = false;
    }

    public void UpdateLevel(int level)
    {
        StartCoroutine(
            AnimationCoroutine.SetPositionVec2Coroutine(this.animationParent, updateOffset, curve, 0.5f)
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
            StartCoroutine(LevelCompleteScreen());
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
                foreach (Image image in _gameStateImage)
                {
                    image.fillAmount = 0;
                }
                StartCoroutine(IncomePhase());
                break;
            case GamePhase.HexPlacement:
                StartCoroutine(FillGameStateImage(_gameStateImage[1], 1));
                _levelText.text = "Hex Placment Phase";
                break;
            case GamePhase.Build:
                StartCoroutine(FillGameStateImage(_gameStateImage[2], 1));
                _levelText.text = "Build Phase";
                _nextLevelButtonCanvasGroup.blocksRaycasts = true;
                StartCoroutine(
                    AnimationCoroutine.FadeCanvasGroup(1, _nextLevelButtonCanvasGroup, 1, 1)
                );
                break;
        }
    }

    private IEnumerator LevelCompleteScreen()
    {
        
        if (GameManager.Instance.CurrentLevel.lifeCurrency > 0)
            LevelCompleteCurrencyAnimationParent[0].SetTempCurrencyText(GameManager.Instance.CurrentLevel.lifeCurrency, CurrencyType.LifeCurrency);
        if (GameManager.Instance.CurrentLevel.hexCurrency > 0)
            LevelCompleteCurrencyAnimationParent[1].SetTempCurrencyText(GameManager.Instance.CurrentLevel.hexCurrency, CurrencyType.HexCurrency);
        if (GameManager.Instance.CurrentLevel.goldCurrency > 0)
            LevelCompleteCurrencyAnimationParent[2].SetTempCurrencyText(GameManager.Instance.CurrentLevel.goldCurrency, CurrencyType.GoldCurrency);

        StartCoroutine(AnimationCoroutine.FadeCanvasGroup(0.3f, _levelCompleteCanvasGroup, 1));
        levelCompleteAnimationParent.anchoredPosition = new Vector2(0, -100);
        StartCoroutine(AnimationCoroutine.SetPositionVec2Coroutine(levelCompleteAnimationParent, new Vector2(0,0), _levelCompleteCurve, 0.5f, 0.1f));

        yield return new WaitForSeconds(0.3f);

        if (GameManager.Instance.CurrentLevel.lifeCurrency > 0)
            LevelCompleteCurrencyAnimationParent[0].NullCurrency(CurrencyType.LifeCurrency);
        else
            LevelCompleteCurrencyAnimationParent[0].LocalRect.gameObject.SetActive(false);
        if (GameManager.Instance.CurrentLevel.hexCurrency > 0)
            LevelCompleteCurrencyAnimationParent[1].NullCurrency(CurrencyType.HexCurrency);
        else
            LevelCompleteCurrencyAnimationParent[1].LocalRect.gameObject.SetActive(false);

        if (GameManager.Instance.CurrentLevel.goldCurrency > 0)
            LevelCompleteCurrencyAnimationParent[2].NullCurrency(CurrencyType.GoldCurrency);
        else
            LevelCompleteCurrencyAnimationParent[1].LocalRect.gameObject.SetActive(false);

        StartCoroutine(WaitForNullCurrency());
    }

    private IEnumerator WaitForNullCurrency()
    {
        bool allInactive = false;
        while (!allInactive)
        {
            allInactive = true;
            foreach (var parent in LevelCompleteCurrencyAnimationParent)
            {
                if (parent.CurrentCurrencyAmount>0)
                {
                    allInactive = false;
                    break;
                }
            }
            yield return null; 
        }
        GameManager.Instance.SetGamePhase(GamePhase.SelectUpgrade);
        StartCoroutine(AnimationCoroutine.FadeCanvasGroup(1, _levelCompleteCanvasGroup, 0, 1f));
    }

    private IEnumerator IncomePhase()
    {
        StartCoroutine(FillGameStateImage(_gameStateImage[0], 1));
        _levelTitleText.color = new Color(1, 1, 1, 0);
        _levelText.text = "Income Phase";
        AnimationCoroutine.FadeCanvasGroup(1, _gameStateCanvasGroup, 1, 0);

        yield return new WaitForSeconds(3);
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
    if (GameManager.Instance.GamePhase == GamePhase.Build)
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
