using System;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{

    [SerializeField] private Button _startButton;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Transform _menuPanel;
    [Header("Animation")]
    [SerializeField] private float animationDuration;
    [SerializeField] private Vector3 animationOffset;
    [SerializeField] private AnimationCurve _animationCurve;
    

    private void Awake()
    {
        _startButton.onClick.AddListener(() => StartGame());
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        OpenMenu();
    }

    private void StartGame()
    {
        _canvasGroup.interactable = false;
        StartCoroutine(AnimationCoroutine.AnimatePositionCoroutine( _menuPanel, animationOffset , 0, animationDuration, _animationCurve));
        StartCoroutine(AnimationCoroutine.FadeCanvasGroup( 0.3f, _canvasGroup, 0, animationDuration/2));
        GameManager.Instance.OnStartGame?.Invoke();
    }

    public void OpenMenu()
    {
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        StartCoroutine(AnimationCoroutine.FadeCanvasGroup( 0.3f, _canvasGroup, 1, 0));
    }

}
