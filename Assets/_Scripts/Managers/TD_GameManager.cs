using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TD_GameManager : MonoBehaviour, I_TDSaveCoordinator, I_RefreshOnSceneChange
{
    public static TD_GameManager instance;

    #region Members
    public bool useSaveData = false;

    private int coreHealth = 5;
    private int currentWaveIndex = -1;
    private int totalWaves = 0;
    private int currentCurrency = 0;
    [SerializeField] private int startingCurrency = 20;

    public int CurrentCurrency { get => currentCurrency; }
    public int CurrentWaveIndex { get => currentWaveIndex; }
    public int TotalWaves { get => totalWaves; }
    public int CoreHealth { get => coreHealth; }

    /// <summary>
    /// Whether or not the Wave is ready to go; will shift to true when user toggles wave to start
    /// </summary>
    private bool playerReady = false;
    /// <summary>
    /// Will be false when a wave completes, true when the user clicks start button
    /// </summary>
    public bool PlayerReady { get => playerReady; }
    public bool HasStarted { get => (currentWaveIndex >= 0); }
    private bool waitingForStart = true;
    public bool IsWaitingForStart { get => (!this.playerReady && !TD_EnemyManager.instance.WaveActive); }

    [SerializeField] private GameObject effectsBin;
    public GameObject EffectsBin { get => effectsBin; }

    GameState gameState;
    #endregion

    protected enum GameState
    {
        MainMenu,
        Loading,
        SceneInit,
        WaveActive,
        Hold,
        Win,
        Lose
    }
    public enum GameSpeedOptions
    {
        PAUSE,
        NORMAL,
        FAST,
        FASTER,
        FASTEST
    }


    #region Lifecycle
    //private void Awake()
    //{
    //    // TODO: This seems to be calling the methods while registering
    //    EventManager.OnEnemyPass += TookDmg;
    //    EventManager.OnWaveFinish += WaveFinished;
    //    EventManager.OnMoneySpent += OnPlayerSpend;
    //    currentWaveIndex = 0;
    //    if (TD_EnemyManager.current) totalWaves = TD_EnemyManager.current.GetTotalWaves();
    //}

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChange;
        SceneManager.sceneLoaded += OnSceneLoad;
        // TODO: This seems to be calling the methods while registering
        EventManager.OnEnemyPass += TookDmg;
        EventManager.OnWaveFinish += WaveFinished;
        EventManager.OnWaveStart += WaveStarted;
        EventManager.OnMoneySpent += OnPlayerSpend;
        currentWaveIndex = 0;
        if (TD_EnemyManager.instance) totalWaves = TD_EnemyManager.instance.GetTotalWaves();
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
        SceneManager.sceneLoaded -= OnSceneLoad;
        SceneManager.activeSceneChanged -= OnSceneChange;
        EventManager.OnEnemyPass -= TookDmg;
        EventManager.OnWaveFinish -= WaveFinished;
        EventManager.OnWaveStart -= WaveStarted;
        EventManager.OnMoneySpent -= OnPlayerSpend;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            //if (useSaveData) TD_GameSerializer.LoadGame();
            //else
            //{
            gameState = GameState.MainMenu;
            currentCurrency = startingCurrency;
            currentWaveIndex = 0;
            //}
            if (!effectsBin) effectsBin = gameObject;
            DontDestroyOnLoad(instance);
        }
        else Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (totalWaves == 0) GetTotalWaves();
        if (coreHealth <= 0) GameOver();
    }
    #endregion

    #region Events
    private void WaveFinished(int ctx)
    {
        if (!HasStarted) return;  
        NextWave();
        //if (!playerReady) return;
        //if (!TD_EnemyManager.instance || TD_EnemyManager.instance.TotalWaves < 1) return;
        //// We may have more than one spawner contributing to the wave, make sure all are done first
        //if (ctx == currentWaveIndex && TD_EnemyManager.instance.IsCurrentWaveComplete()
        //    && playerReady)
        //    NextWave();
        //// Any additonal animations, etc?
        //// EX: "LAST WAVE!" indicator or perhaps dialogue events?
        TD_UIManager.instance.UpdateDisplay();
    }
    private void WaveStarted(int ctx)
    {
        //if (!TD_EnemyManager.instance || TD_EnemyManager.instance.TotalWaves < 1) return;

        ////if (playerReady) PlayerStart();
        //// We may have more than one spawner contributing to the wave, make sure all are done first
        //if (ctx == currentWaveIndex && !TD_EnemyManager.instance.WaveActive && playerReady)
        //    NextWave();
        // Any additonal animations, etc?
        // EX: "LAST WAVE!" indicator or perhaps dialogue events?
        TD_UIManager.instance.UpdateDisplay();
    }
    private void OnPlayerSpend(int ctx)
    {
        // TODO: do we need to check value here for anything ? popup for not being able to afford n resetting? debts? 
        SpendMoney(ctx);
    }
    #endregion

    #region Public
    public void RestartWaves()
    {
        currentWaveIndex = 0;
    }

    public static void SetGameSpeed(GameSpeedOptions gameSpeedRequest)
    {
        switch (gameSpeedRequest)
        {
            case GameSpeedOptions.PAUSE:
            Time.timeScale = 0;
            break;

            case GameSpeedOptions.NORMAL:
            Time.timeScale = 1;
            break;

            case GameSpeedOptions.FAST:
            Time.timeScale = 2;
            break;

            case GameSpeedOptions.FASTER:
            Time.timeScale = 4;
            break;

            case GameSpeedOptions.FASTEST:
            Time.timeScale = 8;
            break;

            default: break;
        }
    }
    public void AddCoins(int deathReward)
    {
        currentCurrency += deathReward;
        // TODO: Check for max? check for quest conditions, etc?
    }

    public bool CanAfford(int purchaseCost)
    {
        return currentCurrency >= purchaseCost;
    }


    public void PlayerStart()
    {
        playerReady = true;
        NextWave();
    }

    public bool SpendMoney(int purchaseCost)
    {
        if (currentCurrency - purchaseCost < 0) return false;
        currentCurrency -= purchaseCost;
        Debug.Log("Current Money:" + currentCurrency.ToString());
        return true;
    }


    #endregion

    #region Private

    private void ToggleSubManagers(bool toState)
    {
        if (TD_BuildManager.instance) TD_BuildManager.instance.gameObject.SetActive(toState);
        if (TD_UIManager.instance) TD_UIManager.instance.gameObject.SetActive(true);
    }
    private void GetTotalWaves()
    {
        if (TD_EnemyManager.instance) totalWaves = TD_EnemyManager.instance.GetTotalWaves();
    }

    private void GameOver()
    {
        EventManager.instance.Lose();
    }

    private void NextWave()
    {
        
        playerReady = false;
        currentWaveIndex++;
        if (currentWaveIndex >= TD_EnemyManager.instance.TotalWaves)
        {
            currentWaveIndex = TD_EnemyManager.instance.TotalWaves;
            EventManager.instance.Win();
        }
    }
    private void TookDmg(int coreDmg)
    {
        coreHealth -= coreDmg;
        TD_UIManager.instance.UpdateDisplay();
    }
    #endregion

    #region Interface
    public void InitFromData(SaveData saveData)
    {
        currentCurrency = saveData.playerMoney;
        currentWaveIndex = saveData.currentWaveIndex;
    }

    public void AddToSaveData(ref SaveData saveData)
    {
        saveData.playerMoney = CurrentCurrency;
        saveData.currentWaveIndex = CurrentWaveIndex;
    }

    public void OnSceneChange(Scene current, Scene next)
    {
        if (TD_BuildManager.instance &&
            current.name != SceneLoader.SceneToName(SceneLoader.GameScene.MainMenu) &&
            current.name != SceneLoader.SceneToName(SceneLoader.GameScene.Settings))
            ToggleSubManagers(false);
    }

    public void OnSceneLoad(Scene current, LoadSceneMode loadSceneMode)
    {
        if (useSaveData && TD_BuildManager.instance &&
            current.name != SceneLoader.SceneToName(SceneLoader.GameScene.MainMenu) &&
            current.name != SceneLoader.SceneToName(SceneLoader.GameScene.Settings) )
        {
            ToggleSubManagers(true);
            TD_GameSerializer.LoadGame();
        }
        ReInit();
    }

    public void ReInit()
    {
        if (!useSaveData) currentCurrency = startingCurrency;
    }
    #endregion
}
