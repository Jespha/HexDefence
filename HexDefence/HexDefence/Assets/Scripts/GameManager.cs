using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public UIManager UIManager;

    public EnemyManager EnemyManager;

    public CameraManager CameraManager;

    public PlayerInput PlayerInput;

    public GameObject FollowTarget;

    [field: Header ("Levels")]
    [SerializeField]
    public Levels Levels { get; private set; }
    public Level CurrentLevel { get; private set; }

    [field:Header("Selection Modes")]
    public bool Buildmode { get; private set; }
    public Action OnBuildmode;
    public HexBuilding TempBuilding { get; private set; }
    public bool BuildHexmode { get; private set; } //TODO: Might use this later to show all possible Hexes to build on
    public bool MultiBuildMode { get; private set; }
    public bool UpgradeMode { get; private set; }

    [field:Header("Upgrades")]
    public static UpgradesUnlocked UpgradesUnlockedInstance;
    public int UpgradesToAdd = 1;

    [field:Header("Game Events")]
    public Action<int,Level> OnLevelStart;
    public Action<int,Level> OnLevelComplete;
    public Action<GamePhase> UpdateGamePhase;
    public Action NoMoreLives;
    public GamePhase GamePhase = GamePhase.Income;

    void Awake() {
        Instance = this;
        Levels = Resources.Load<Levels>("ScriptableObjects/LEVELS/LEVELS");
        UpgradesUnlockedInstance = Resources.Load<UpgradesUnlocked>("ScriptableObjects/Upgrade/UpgradesUnlocked");
    }


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
        PlayerInput.MultiBuildMode += SetMultiBuildMode;
        PlayerInput.UpgradeBuildMode += SetUpgradeMode;
    }

    private void Update()
    {
        // Update game phase Logic INCOME -> HEXPLACEMENT -> BUILD -> DEFEND
        // HEXPLACMENT -> BUILD 
        if (GamePhase == GamePhase.HexPlacement)
        {
            if (Currency.Instance.HexCurrency == 0)
            {
                SetGamePhase(GamePhase.Build);
            }
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
        StartCoroutine(StartGameCoroutine());
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

    private IEnumerator StartGameCoroutine()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
        yield return new WaitForSeconds(1); 

        UpgradesUnlockedInstance.InitializeCurrentUpgradesUnlocked();
        CurrentLevel = Levels.LevelList[0];
        Currency.Instance.UpdateCurrency(25, CurrencyType.MaxLifeCurrency, AnimationCoroutine.RectTransformToScreenSpace(UIManager.levelDisplay.LevelCompleteCurrencyAnimationParent[0].LocalRect).position);
        StartCoroutine(LevelComplete());
    }

    public IEnumerator LevelComplete()
    {   
        UpdateUpgradesToAdd(1); //TODO: Make this dynamic or at least get from level scriptable object

        OnLevelComplete?.Invoke(Levels.LevelList.IndexOf(CurrentLevel), CurrentLevel);
        GamePhase = GamePhase.Income;
        UpdateGamePhase?.Invoke(GamePhase);
        Currency.Instance.UpdateCurrency(CurrentLevel.lifeCurrency, CurrencyType.LifeCurrency,
        AnimationCoroutine.RectTransformToScreenSpace(UIManager.levelDisplay.LevelCompleteCurrencyAnimationParent[0].LocalRect).position);
        Currency.Instance.UpdateCurrency(CurrentLevel.hexCurrency, CurrencyType.HexCurrency,
        AnimationCoroutine.RectTransformToScreenSpace(UIManager.levelDisplay.LevelCompleteCurrencyAnimationParent[1].LocalRect).position);
        Currency.Instance.UpdateCurrency(CurrentLevel.goldCurrency, CurrencyType.GoldCurrency,
        AnimationCoroutine.RectTransformToScreenSpace(UIManager.levelDisplay.LevelCompleteCurrencyAnimationParent[2].LocalRect).position);;
        yield return null;
    }

    public void SetGamePhase(GamePhase gamePhase)
    {
        GamePhase = gamePhase;
        UpdateGamePhase?.Invoke(gamePhase);
        Debug.Log("GamePhase: " + gamePhase);
    }

    // make event to call this from button
    // also activate button to call event
    public void LoadLevelIfPossible()
    {
        if (GamePhase == GamePhase.Build)
        {
            if (Levels.LevelList.IndexOf(CurrentLevel) != 0)
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

    public void SetBuildMode(bool buildMode, HexBuilding hexBuilding = null)
    {
        if (hexBuilding == null || buildMode == false)
        {
            Buildmode = false;
            hexBuilding = null;
            OnBuildmode?.Invoke();
           return;
        }
        Buildmode = buildMode;
        TempBuilding = hexBuilding;
        OnBuildmode?.Invoke();
    }

    public void SetBuildHexMode(bool buildHexMode)
    {
        BuildHexmode = buildHexMode;
    }

    private void SetMultiBuildMode(bool multiBuildMode)
    {
        MultiBuildMode = multiBuildMode;
    }

    private void SetUpgradeMode(bool upgradeMode)
    {
        UpgradeMode = upgradeMode;
    }

    public void UpdateUpgradesToAdd(int amount)
    {
        UpgradesToAdd += amount;
    }

}

public enum GamePhase
{
    Income,
    SelectUpgrade,
    HexPlacement,
    Build,
    Defend
}