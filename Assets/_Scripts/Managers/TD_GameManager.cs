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

    public void WipeWave()
    {
        // TODO: Guardrails?
        TD_EnemyManager.instance.ClearAndEndWave();
    }

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
    public bool IsWaitingForStart { get => (!this.playerReady && !TD_EnemyManager.instance.SpawnersActive); }

    [SerializeField] private GameObject effectsBin;
    public GameObject EffectsBin { get => effectsBin; }

    GameState gameState;
    #endregion
    protected Coroutine stateTransition;
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
        EventManager.OnPlayerReady += OnPlayerReady;
        EventManager.OnMoneySpent += OnPlayerSpend;
        currentWaveIndex = 0;
        if (TD_EnemyManager.instance) totalWaves = TD_EnemyManager.instance.GetTotalWaves();
        TryChangeState(GameState.SceneInit);
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
        SceneManager.sceneLoaded -= OnSceneLoad;
        SceneManager.activeSceneChanged -= OnSceneChange;
        EventManager.OnEnemyPass -= TookDmg;
        EventManager.OnWaveFinish -= WaveFinished;
        EventManager.OnWaveStart -= WaveStarted;
        EventManager.OnPlayerReady -= OnPlayerReady;
        EventManager.OnMoneySpent -= OnPlayerSpend;
    }

    private void OnPlayerReady()
    {
        playerReady = true;
        if (TD_EnemyManager.instance.IsCurrentWaveGroupComplete()) IncrementAndCheckWin();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;

            gameState = GameState.MainMenu;
            currentCurrency = startingCurrency;
            currentWaveIndex = 0;

            if (!effectsBin) effectsBin = gameObject;
            DontDestroyOnLoad(instance);
        }
        else Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (totalWaves == 0) GetTotalWaves();
        if (coreHealth <= 0) TryChangeState(GameState.Lose);
    }
    #endregion

    #region Events
    /// <summary>
    /// GM will poll the EM to see if ALL of the waves have finished. If so, we wait for input
    /// </summary>
    /// <param name="ctx"></param>
    private void WaveFinished(int ctx)
    {
        if (!HasStarted) return;
        //// We may have more than one spawner contributing to the wave, make sure all are done first
        //if (TD_EnemyManager.instance.IsCurrentWaveComplete()) NextWave();
        //// Any additonal animations, etc?
        //// EX: "LAST WAVE!" indicator or perhaps dialogue events?

        waitingForStart = TD_EnemyManager.instance.IsCurrentWaveGroupComplete();
        if (gameState == GameState.WaveActive && waitingForStart) TryChangeState(GameState.Hold);
        if (ctx >= TD_EnemyManager.instance.TotalWaves - 1) TryChangeState(GameState.Win);
        TD_UIManager.instance.UpdateDisplay();
    }

    private void WaveStarted(int ctx)
    {
        if (!TD_EnemyManager.instance || TD_EnemyManager.instance.TotalWaves < 1) return;
        TryChangeState(GameState.WaveActive);

        //// We may have more than one spawner contributing to the wave, make sure all are done first
        //if (ctx == currentWaveIndex && TD_EnemyManager.instance.IsCurrentWaveGroupComplete())
        //    IncrementAndCheckWin();
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
    private void IncrementAndCheckWin()
    {
        playerReady = false;
        currentWaveIndex++;
        if (currentWaveIndex >= TD_EnemyManager.instance.TotalWaves) TryChangeState(GameState.Win);
        else EventManager.OnWaveStart(currentWaveIndex);
    }

    private void TookDmg(int coreDmg)
    {
        coreHealth -= coreDmg;
        TD_UIManager.instance.UpdateDisplay();
    }

    private void TryChangeState(GameState toState = GameState.Hold)
    {
        if (gameState == GameState.Win || gameState == GameState.Lose) return;
        switch (toState)
        {
            case GameState.MainMenu:

            break;

            case GameState.Loading:
                // TODO: Loading screen? 
            Debug.Log("LOADING");
            break;

            case GameState.SceneInit:
            waitingForStart = true;
            playerReady = false;
            break;

            case GameState.WaveActive:
            waitingForStart = false;
            playerReady = false;
            break;

            case GameState.Hold:
            waitingForStart = true;
            break;

            case GameState.Win:
            EventManager.instance.Win();
            break;

            case GameState.Lose:
            EventManager.instance.Lose();
            break;

            default:
            SafeTransition(GameState.SceneInit, 0.5f);
            break;
        }
        gameState = toState;
    }

    /// <summary>
    /// Provides a delayed state shift and wrapped with verifying coroutines
    /// </summary>
    /// <param name="nextState"></param>
    /// <param name="delay"></param>
    protected void SafeTransition(GameState nextState, float delay)
    {
        // Setup transition back to moving
        if (stateTransition != null) { StopCoroutine(stateTransition); stateTransition = null; }
        stateTransition = StartCoroutine(StateTransition(nextState, delay));
    }
    protected IEnumerator StateTransition(GameState nextState, float delay)
    {
        yield return new WaitForSeconds(delay);
        TryChangeState(nextState);
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
        if (EventManager.instance) EventManager.instance.TowerDeselected();
        //if (TD_BuildManager.instance &&
        //    next.name != SceneLoader.SceneToName(SceneLoader.GameScene.MainMenu) &&
        //    next.name != SceneLoader.SceneToName(SceneLoader.GameScene.Settings))
            //ToggleSubManagers(false);
    }

    public void OnSceneLoad(Scene current, LoadSceneMode loadSceneMode)
    {
        if (useSaveData && TD_BuildManager.instance &&
            current.name != SceneLoader.SceneToName(SceneLoader.GameScene.MainMenu) &&
            current.name != SceneLoader.SceneToName(SceneLoader.GameScene.Settings) )
        {
            ToggleSubManagers(true);
            TD_AudioManager.instance.PlayMusic(TD_AudioManager.instance.BasicLevelMusic);
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
