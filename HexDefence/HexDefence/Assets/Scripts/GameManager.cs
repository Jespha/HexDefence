using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    public UIManager UIManager;

    [SerializeField]
    public EnemyManager EnemyManager;

    [SerializeField]
    public CameraManger CameraManager;

    [field: SerializeField]
    public Levels Levels { get; private set; }

    [SerializeField]
    public Level CurrentLevel { get; private set; }
    private bool _noMoreEnemies = false;
    private bool _canGotoNextLevel = false;

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
                CameraManager = FindObjectOfType<CameraManger>();
            }
            catch
            {
                Debug.Log("CameraManager not found in GameManager");
            }
        }
    }

    private void Update()
    {
        if (_noMoreEnemies && Currency.Instance.HexCurrency == 0)
        {
            _canGotoNextLevel = true;
            _noMoreEnemies = false;
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
        EnemyManager.SpawnEnemies(CurrentLevel);
    }

    private void LoadNextLevel()
    {
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
    }

    public void GameOver()
    {
        EnemyManager.ClearEnemies();
        HexGridManager.Instance.MakeHexGrid();
        RoadManager.Instance.ClearRoads();
        StartGame();
    }
}
