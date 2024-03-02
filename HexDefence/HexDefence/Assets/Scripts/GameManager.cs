using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    public UIManager UIManager;

    [SerializeField]
    public EnemyManager EnemyManager;

    [SerializeField]
    public CameraManager CameraManager;

    [SerializeField]
    public PlayerInput PlayerInput;

    [field: SerializeField]
    public Levels Levels { get; private set; }

    [SerializeField]
    public GameObject FollowTarget;

    public Level CurrentLevel { get; private set; }
    private bool _noMoreEnemies = false;
    private bool _canGotoNextLevel = false;

    /// GLOBAL GAME EVENTS
    public Action OnLevelStart;
    public Action OnLevelComplete;
    public Action NoMoreLives;

    void Awake() => Instance = this;


    private void Start()
    {
        if (UIManager == null)
        {
            try
            {
                UIManager = FindObjectOfType<UIManager>();
            }
            catch
            {
                Debug.Log("UIManager not found in GameManager");
            }
        }

        if (CameraManager == null)
        {
            try
            {
                CameraManager = FindObjectOfType<CameraManager>();
            }
            catch
            {
                Debug.Log("CameraManager not found in GameManager");
            }
        }

        if (PlayerInput == null)
        {
            try
            {
                PlayerInput = FindObjectOfType<PlayerInput>();
            }
            catch
            {
                Debug.Log("PlayerInput not found in GameManager");
            }
        }

        StartGame();
    }

    private void Update()
    {
        if (_noMoreEnemies && Currency.Instance.HexCurrency == 0)
        {
            _canGotoNextLevel = true;
            _noMoreEnemies = false;
        }

        if (Currency.Instance.LifeCurrency > 0)
        {
            PlayerInput.HandleAllInputs();
        }
        else
        {
           FollowTarget.transform.position = new Vector3(0, 0, 0);
        }

    }

    public void StartGame()
    {
        CurrentLevel = Levels.LevelList[0];
        Currency.Instance.UpdateCurrency(25, CurrencyType.LifeCurrency);
        Currency.Instance.UpdateCurrency(25, CurrencyType.MaxLifeCurrency);
        Currency.Instance.UpdateCurrency(CurrentLevel.hexCurrency, CurrencyType.HexCurrency);
        Currency.Instance.UpdateCurrency(CurrentLevel.goldCurrency, CurrencyType.GoldCurrency);
        UIManager.SetLevel(0);
    }

    public void LoadNextLevel()
    {
        OnLevelStart?.Invoke();
        if (Levels.LevelList.IndexOf(CurrentLevel) + 1 < Levels.LevelList.Count)
        {
            CurrentLevel = Levels.LevelList[Levels.LevelList.IndexOf(CurrentLevel) + 1];
            Currency.Instance.UpdateCurrency(CurrentLevel.hexCurrency, CurrencyType.HexCurrency);
            Currency.Instance.UpdateCurrency(CurrentLevel.goldCurrency, CurrencyType.GoldCurrency);
            UIManager.SetLevel(Levels.LevelList.IndexOf(CurrentLevel));
            EnemyManager.SpawnEnemies(CurrentLevel);
        }
        else
        {
            Debug.Log("No more levels");
        }
    }

    // make event to call this from button
    // also activate button to call event
    public void GotoNextLevel()
    {
        if (_canGotoNextLevel)
        {
            _canGotoNextLevel = false;
            LoadNextLevel();
        }
    }

    public void NoMoreEnemies()
    {
        _noMoreEnemies = true;
        UIManager.SetLevelComplete(Levels.LevelList.IndexOf(CurrentLevel));
    }

    public void GameOver()
    {
        EnemyManager.ClearEnemies();
        HexGridManager.Instance.MakeHexGrid();
        RoadManager.Instance.ClearRoads();
        StartGame();
    }

    public void SetFollowTarget(HexCell hexCell)
    {
        FollowTarget.transform.position = new Vector3(hexCell.transform.position.x, FollowTarget.transform.position.y, -FollowTarget.transform.position.y + hexCell.transform.position.z);
    }
}
