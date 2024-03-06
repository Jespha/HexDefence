using System;
using System.Collections;
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

    /// GLOBAL GAME EVENTS
    public Action<int,Level> OnLevelStart;
    public Action<int,Level> OnLevelComplete;
    public Action<GamePhase> UpdateGamePhase;
    public Action NoMoreLives;
    public GamePhase GamePhase = GamePhase.Income;

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
        // if (_noMoreEnemies && Currency.Instance.HexCurrency == 0)
        // {
        //     _canGotoNextLevel = true;
        //     _noMoreEnemies = false;
        // }

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
        Currency.Instance.UpdateCurrency(25, CurrencyType.LifeCurrency);
        Currency.Instance.UpdateCurrency(25, CurrencyType.MaxLifeCurrency);
        CurrentLevel = Levels.LevelList[0];
        StartCoroutine(LevelComplete());
    }

    public void LoadNextLevel()
    {
        if (Levels.LevelList.IndexOf(CurrentLevel) + 1 < Levels.LevelList.Count)
        {
            CurrentLevel = Levels.LevelList[Levels.LevelList.IndexOf(CurrentLevel) + 1];
            OnLevelStart?.Invoke(Levels.LevelList.IndexOf(CurrentLevel), CurrentLevel);
        }
        else
        {
            Debug.Log("No more levels");
        }
    }

    public IEnumerator LevelComplete()
    {   
        yield return new WaitForSeconds(1); //TODO: Make it wait for initialization of all managers
        OnLevelComplete?.Invoke(Levels.LevelList.IndexOf(CurrentLevel), CurrentLevel);
        GamePhase = GamePhase.Income;
        UpdateGamePhase?.Invoke(GamePhase);
        yield return new WaitForSeconds(3);
        Currency.Instance.UpdateCurrency(CurrentLevel.hexCurrency, CurrencyType.HexCurrency);
        Currency.Instance.UpdateCurrency(CurrentLevel.goldCurrency, CurrencyType.GoldCurrency);
        yield return null;
    }

    public void SetGamePhase(GamePhase gamePhase)
    {
        GamePhase = gamePhase;
        UpdateGamePhase?.Invoke(gamePhase);
    }

    // make event to call this from button
    // also activate button to call event
    public void LoadLevelIfPossible()
    {
        if (GamePhase == GamePhase.Build)
        {
            EnemyManager.ClearEnemies();
            GamePhase = GamePhase.Defend;
            LoadNextLevel();
        }
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
        if(Vector3.Distance(new Vector3(FollowTarget.transform.position.x,hexCell.transform.position.y,hexCell.transform.position.z), hexCell.transform.position) > 16f)
        FollowTarget.transform.position = new Vector3(hexCell.transform.position.x, FollowTarget.transform.position.y, -FollowTarget.transform.position.y + hexCell.transform.position.z);
    }
}

public enum GamePhase
{
    Income,
    HexPlacement,
    Build,
    Defend
}