using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TD_GameManager : MonoBehaviour
{
    public GameObject gameStatusUI;
    public GameObject gameOverDisplay;
    public GameObject waveStartDisplay;
    public int coreHealth = 5;
    private int currentWave = 0;

    private void Awake()
    {
        EventManager.OnEnemyPass += (ctx) => TookDmg(ctx);
        EventManager.OnWaveStart+= (ctx) => WaveStarted(ctx);
        waveStartDisplay.GetComponentInChildren<Button>().onClick.AddListener(() => EventManager.WaveStarted(currentWave));
        UpdateDisplay();
    }

    private void WaveStarted(int ctx)
    {
        currentWave = ctx;
        // Any additonal animations, etc?
        // EX: "LAST WAVE!" indicator or perhaps dialogue events?
        UpdateDisplay();
    }

    private void TookDmg(int coreDmg)
    {
        coreHealth -= coreDmg;
        UpdateDisplay();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (coreHealth <= 0) GameOver();
    }

    private void GameOver()
    {
        UpdateDisplay();
        //gameOverDisplay.SetActive(true);
    }

    private void UpdateDisplay()
    {
        if (gameStatusUI) gameStatusUI.GetComponentInChildren<TMP_Text>().text = coreHealth.ToString();
        if (coreHealth <= 0) gameOverDisplay.SetActive(true);
        if (waveStartDisplay) waveStartDisplay.GetComponentInChildren<TMP_Text>().text = currentWave.ToString();
    }
}
