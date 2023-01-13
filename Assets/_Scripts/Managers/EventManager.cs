using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager current;

    #region Wave Events
    public static UnityAction<int> OnEnemyPass;
    private UnityEvent<int> enemyPassEvent;

    public static UnityAction<int> OnWaveStart;
    private UnityEvent<int> waveStartEvent;

    public static UnityAction<int> OnWaveFinish;
    private UnityEvent<int> waveFinishEvent;
    #endregion


    #region UI Events
    public static UnityAction<TD_Building> OnTowerSelect;
    private UnityEvent<TD_Building> towerSelectEvent;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        if (current != null) Destroy(this);
        current = this;

        enemyPassEvent = new UnityEvent<int>();
        waveStartEvent = new UnityEvent<int>();
        waveFinishEvent = new UnityEvent<int>();
        towerSelectEvent = new UnityEvent<TD_Building>();

        //enemyPassEvent.AddListener((ctx) => OnEnemyPass(ctx));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void EnemyPassedCore(int coreDmg)
    {
        Debug.Log($"Enemy Passed Core: {coreDmg}");
        current.enemyPassEvent.Invoke(coreDmg);
        OnEnemyPass.Invoke(coreDmg);
    }

    /// <summary>
    /// AS / AFTER the wave starts
    /// </summary>
    /// <param name="waveIndex"></param>
    public static void WaveStarted(int waveIndex)
    {
        // Wave finish + timer OR button push for start next
        // TODO: Maybe checkpoints included for user prompt before start? 
        Debug.Log($"Enemy Wave Started: {waveIndex}");
        current.waveStartEvent.Invoke(waveIndex);
        OnWaveStart.Invoke(waveIndex); 
    }

    /// <summary>
    /// After the wave has had all enemies die or complete track
    /// </summary>
    /// <param name="waveIndex"></param>
    public static void WaveFinished(int waveIndex)
    {
        // Enemies finish spawning + died / get to end
        Debug.Log($"Enemy Wave Finished: {waveIndex}");
        current.waveFinishEvent.Invoke(waveIndex);
        OnWaveFinish.Invoke(waveIndex);
    }

    public static void TowerSelected(TD_Building bSelected)
    {
        // Enemies finish spawning + died / get to end
        Debug.Log($"Building Selected: {bSelected}");
        current.towerSelectEvent.Invoke(bSelected);
        OnTowerSelect.Invoke(bSelected);
    }
}
