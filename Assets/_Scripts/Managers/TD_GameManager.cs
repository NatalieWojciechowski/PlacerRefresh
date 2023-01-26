using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_GameManager : MonoBehaviour
{
    public static TD_GameManager current;

    private int coreHealth = 5;
    private int currentWaveIndex = -1;

    private bool spawningEnabled = false;
    public bool HasStarted { get => (this.spawningEnabled && currentWaveIndex >= 0); }

    public TD_UIManager uIManager;

    private int startingCurrency = 20;
    private int currentCurrency = 0;
    public int CurrentCurrency { get => currentCurrency; }
    public int CurrentWaveIndex { get => currentWaveIndex; }
    private int totalWaves = 0;
    public int TotalWaves { get => totalWaves; }
    public int CoreHealth { get => coreHealth; }

    private void Awake()
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
        currentCurrency = startingCurrency;
#if DEBUG
        currentCurrency = 1000;
#endif
        if (!uIManager) uIManager = FindObjectOfType<TD_UIManager>();
        currentWaveIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (totalWaves == 0) GetTotalWaves();
        if (coreHealth <= 0) GameOver();
    }

    private void GetTotalWaves()
    {
        if (TD_EnemyManager.current) totalWaves = TD_EnemyManager.current.GetTotalWaves();
    }

    public void PlayerStart()
    {
        spawningEnabled = true;
    }

    private void GameOver()
    {
        EventManager.current.Lose();
    }

    private void WaveFinished(int ctx)
    {
        if (!spawningEnabled) return;
        if (!TD_EnemyManager.current || TD_EnemyManager.current.TotalWaves < 1) return;
        if (ctx == currentWaveIndex)
            currentWaveIndex++;
        if (currentWaveIndex > totalWaves)
        {
            currentWaveIndex = totalWaves;
            EventManager.current.Win();
        }
        // Any additonal animations, etc?
        // EX: "LAST WAVE!" indicator or perhaps dialogue events?
        uIManager.UpdateDisplay();
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
        FASTER
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
}
