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

    public int CurrentWave { get => currentWave; }
    public int CoreHealth { get => coreHealth; }

    private void Awake()
    {
        EventManager.OnEnemyPass += (ctx) => TookDmg(ctx);
        EventManager.OnWaveStart+= (ctx) => WaveStarted(ctx);
    }
    private void OnDisable()
    {
        EventManager.OnEnemyPass -= (ctx) => TookDmg(ctx);
        EventManager.OnWaveStart -= (ctx) => WaveStarted(ctx);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (current != null) Destroy(this);
        current = this;
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
}
