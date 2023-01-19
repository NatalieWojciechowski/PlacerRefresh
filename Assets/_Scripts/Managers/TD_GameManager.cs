using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_GameManager : MonoBehaviour
{
    public static TD_GameManager current;

    private int coreHealth = 5;
    private int currentWave = 0;

    public TD_UIManager uIManager;

    private int startingCurrency = 20;
    private int currentCurrency = 0;
    public int CurrentCurrency { get => currentCurrency; }
    public int CurrentWave { get => currentWave; }
    public int TotalWaves { get => TD_EnemyManager.current.GetTotalWaves(); }
    public int CoreHealth { get => coreHealth; }

    private void Awake()
    {
        EventManager.OnEnemyPass += (ctx) => TookDmg(ctx);
        EventManager.OnWaveStart+= (ctx) => WaveStarted(ctx);
        EventManager.OnMoneySpent += (ctx) => OnPlayerSpend(ctx);
    }
    private void OnDisable()
    {
        EventManager.OnEnemyPass -= (ctx) => TookDmg(ctx);
        EventManager.OnWaveStart -= (ctx) => WaveStarted(ctx);
        EventManager.OnMoneySpent -= (ctx) => OnPlayerSpend(ctx);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (current != null) Destroy(this);
        current = this;
        currentCurrency = startingCurrency;
        if (!uIManager) uIManager = FindObjectOfType<TD_UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (coreHealth <= 0) GameOver();
    }

    private void GameOver()
    {
        // TODO: Change this to an event
        uIManager.UpdateDisplay();
        //UpdateDisplay();
        ////gameOverDisplay.SetActive(true);
    }
    private void WaveStarted(int ctx)
    {
        currentWave = ctx;
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
