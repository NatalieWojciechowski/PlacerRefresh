using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_GameManager : MonoBehaviour, I_TDSaveCoordinator
{
    public static TD_GameManager current;

    private int coreHealth = 5;
    private int currentWaveIndex = -1;

    /// <summary>
    /// Whether or not the Wave is ready to go; will shift to true when user toggles wave to start
    /// </summary>
    private bool playerReady = false;
    public bool HasStarted { get => (this.playerReady && currentWaveIndex >= 0); }
    private bool waitingForStart = true;
    public bool IsWaitingForStart { get => TD_EnemyManager.current.IsCurrentWaveComplete(); }

    public TD_UIManager uIManager;

    [SerializeField] private int startingCurrency = 20;
    private int currentCurrency = 0;
    public int CurrentCurrency { get => currentCurrency; }
    public int CurrentWaveIndex { get => currentWaveIndex; }
    private int totalWaves = 0;
    public int TotalWaves { get => totalWaves; }
    public int CoreHealth { get => coreHealth; }

    [SerializeField] private GameObject effectsBin;

    public GameObject EffectsBin { get => effectsBin; }

    GameState gameState;
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
        // TODO: This seems to be calling the methods while registering
        EventManager.OnEnemyPass += TookDmg;
        EventManager.OnWaveFinish += WaveFinished;
        EventManager.OnMoneySpent += OnPlayerSpend;
        currentWaveIndex = 0;
        if (TD_EnemyManager.current) totalWaves = TD_EnemyManager.current.GetTotalWaves();
    }

    private void OnDisable()
    {
        EventManager.OnEnemyPass -= TookDmg;
        EventManager.OnWaveFinish -= WaveFinished;
        EventManager.OnMoneySpent -= OnPlayerSpend;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (current != null) Destroy(this);
        current = this;
        gameState = GameState.MainMenu;
        currentCurrency = startingCurrency;
        if (!uIManager) uIManager = FindObjectOfType<TD_UIManager>();
        currentWaveIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (totalWaves == 0) GetTotalWaves();
        if (coreHealth <= 0) GameOver();
    }

    public void RestartWaves()
    {
        currentWaveIndex = 0;
    }

    private void GetTotalWaves()
    {
        if (TD_EnemyManager.current) totalWaves = TD_EnemyManager.current.GetTotalWaves();
    }

    public void PlayerStart()
    {
        playerReady = true;
    }

    private void GameOver()
    {
        EventManager.current.Lose();
    }

    private void WaveFinished(int ctx)
    {
        if (!playerReady) return;
        if (!TD_EnemyManager.current || TD_EnemyManager.current.TotalWaves < 1) return;
        // We may have more than one spawner contributing to the wave, make sure all are done first
        if (ctx == currentWaveIndex && TD_EnemyManager.current.IsCurrentWaveComplete()
            && playerReady)            
            NextWave();
        // Any additonal animations, etc?
        // EX: "LAST WAVE!" indicator or perhaps dialogue events?
        uIManager.UpdateDisplay();
    }

    private void NextWave()
    {
        playerReady = false;
        currentWaveIndex++;
        if (currentWaveIndex > TD_EnemyManager.current.TotalWaves)
        {
            currentWaveIndex = TD_EnemyManager.current.TotalWaves;
            EventManager.current.Win();
        }
    }

    public bool SpendMoney(int purchaseCost)
    {
        if (currentCurrency - purchaseCost < 0) return false;
        currentCurrency -= purchaseCost;
        return true;
    }

    private void TookDmg(int coreDmg)
    {
        coreHealth -= coreDmg;
        uIManager.UpdateDisplay();
    }
    public enum GameSpeedOptions
    {
        PAUSE,
        NORMAL,
        FAST,
        FASTER,
        FASTEST
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

    internal void AddCoins(int deathReward)
    {
        currentCurrency += deathReward;
        // TODO: Check for max? check for quest conditions, etc?
    }

    internal bool CanAfford(int purchaseCost)
    {
        return currentCurrency >= purchaseCost;
    }

    private void OnPlayerSpend(int ctx)
    {
        // TODO: do we need to check value here for anything ? popup for not being able to afford n resetting? debts? 
        SpendMoney(ctx);
    }


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
    #endregion
}
