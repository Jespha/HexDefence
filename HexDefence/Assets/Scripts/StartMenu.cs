using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
	[SerializeField]
	private Button _startButton;

	[SerializeField]
	private CanvasGroup _canvasGroup;

	[SerializeField]
	private Transform _menuPanel;

	[Header("Animation")]
	[SerializeField]
	private float animationDuration;

	[SerializeField]
	private Vector3 animationOffset;

	[SerializeField]
	private AnimationCurve _animationCurve;
	private bool _firstTime = true;

	[SerializeField]
	private bool _skipMenu = false;

	private async void Awake()
	{
		_startButton.onClick.AddListener(() => StartGame());
		_canvasGroup.interactable = false;
		_canvasGroup.blocksRaycasts = false;
		OpenMenu(_firstTime);
		if (_skipMenu)
		{
			await StartGame();
		}
	}

	private async Task StartGame()
	{
		await Task.Delay(TimeSpan.FromSeconds(0.5f));
		_canvasGroup.interactable = false;
		StartCoroutine(
			AnimationCoroutine.AnimatePositionCoroutine(
				_menuPanel,
				animationOffset,
				0,
				animationDuration,
				_animationCurve
			)
		);
		StartCoroutine(
			AnimationCoroutine.FadeCanvasGroup(0.3f, _canvasGroup, 0, animationDuration / 2)
		);
		await Task.Delay(TimeSpan.FromSeconds(0.3f));
		GameManager.Instance.OnStartGame?.Invoke();
		// await Task.Delay(TimeSpan.FromSeconds(animationDuration));
		// OpenMenu();
		// StartCoroutine(AnimationCoroutine.AnimatePositionCoroutine( _menuPanel, -animationOffset , 0, animationDuration, _animationCurve));
		// GameManager.Instance.BackToMenu();
	}

	public void OpenMenu(bool _firstTime = false)
	{
		_canvasGroup.interactable = true;
		_canvasGroup.blocksRaycasts = true;
		StartCoroutine(AnimationCoroutine.FadeCanvasGroup(0.3f, _canvasGroup, 1, 0));
		if (!_firstTime)
			StartCoroutine(
				AnimationCoroutine.AnimatePositionCoroutine(
					_menuPanel,
					-animationOffset,
					0,
					animationDuration,
					_animationCurve
				)
			);
	}
}
