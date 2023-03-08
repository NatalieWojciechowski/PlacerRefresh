using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    #region Wave Events
    public static UnityAction<int> OnEnemyPass;
    private CustomIntEvent enemyPassEvent;

    public static UnityAction<int> OnWaveStart;
    private CustomIntEvent waveStartEvent;

    public static UnityAction<int> OnWaveFinish;
    private CustomIntEvent waveFinishEvent;
    #endregion
    //public static Action<int> WaveFinishedAction = (ctx) => { OnWaveFinish(ctx); };
    //private Event waveFinishEvent;

    #region CustomEvents
    [System.Serializable]
    public class CustomIntEvent : UnityEvent<int>
    {

    }

    [System.Serializable]
    public class CustomBuildingEvent : UnityEvent<TD_Building>
    {

    }

    [System.Serializable]
    public class CustomArgsEvent : UnityEvent<CustomEventArgs>
    {

    }

    [System.Serializable]
    public class CustomEventArgs : EventArgs
    {
        public TD_Events tdEvents { get; set; }
    }

    #endregion

    #region UI Events
    public static UnityAction<TD_Building> OnTowerSelect;
    private CustomBuildingEvent towerSelectEvent;

    public static UnityAction OnTowerDeselect;
    private Event towerDeselectEvent;

    public static UnityAction<int> OnMoneySpent;
    private CustomIntEvent moneySpentEvent;

    public static event EventHandler GameOver;
    public static event EventHandler GameWon;

    public delegate void OnGodModeToggleEvent();
    public static event EventHandler OnGodModeToggle;


    public enum TD_Events
    {
        GenericCancel,
        GenericAccept,
    }

    //public static Action<CustomEventArgs> A_GenericCancel;
    //public static Action<CustomEventArgs> A_GenericAccept;
    public static event EventHandler<CustomEventArgs> CancelTriggered;
    public static CustomArgsEvent OnCancel;
    public static CustomArgsEvent OnAccept;

    //public delegate void TD_EventHandler(object source, EventArgs args);
    //public event TD_EventHandler EventTriggered;
    //public static event Action<TD_Events> tdEventTriggered;


    public static UnityAction<TD_Building> OnTowerBlueprint;
    public static UnityAction<TD_Building> OnTowerPlace;




    //private CustomBuildingEvent towerSelectEvent;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else Destroy(this);

        ////enemyPassEvent = new();
        ////waveStartEvent = new();
        ////waveFinishEvent = new();
        ////towerSelectEvent = new();
        ////towerDeselectEvent = new();
        ////moneySpentEvent = new();
        //DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void EnemyPassedCore(int coreDmg)
    {
        Debug.Log($"Enemy Passed Core: {coreDmg}");
        //current.enemyPassEvent.Invoke(coreDmg);
        OnEnemyPass?.Invoke(coreDmg);
    }

    /// <summary>
    /// AS / AFTER the wave starts
    /// </summary>
    /// <param name="waveIndex"></param>
    public void WaveStarted(int waveIndex)
    {
        //if (!TD_GameManager.instance.PlayerReady) TD_GameManager.instance.PlayerStart();
        // Wave finish + timer OR button push for start next
        // TODO: Maybe checkpoints included for user prompt before start? 
        Debug.Log($"Enemy Wave Started: {waveIndex}");
        //current.waveStartEvent.Invoke(waveIndex);
        OnWaveStart?.Invoke(waveIndex); 
    }

    /// <summary>
    /// After the wave has had all enemies die or complete track
    /// </summary>
    /// <param name="waveIndex"></param>
    public void WaveFinished(int waveIndex)
    {
        if (!TD_GameManager.instance.HasStarted) return;
        // Enemies finish spawning + died / get to end
        Debug.Log($"Enemy Wave Finished: {waveIndex}");
        //current.waveFinishEvent.Invoke(waveIndex);
        OnWaveFinish?.Invoke(waveIndex);
    }

    public void TowerSelected(TD_Building bSelected)
    {
        // Enemies finish spawning + died / get to end
        Debug.Log($"Building Selected: {bSelected}");
        //current.towerSelectEvent.Invoke(bSelected);
        OnTowerSelect?.Invoke(bSelected);
    }

    public void TowerDeselected()
    {
        // Enemies finish spawning + died / get to end
        Debug.Log($"Building Deselected: ");
        //current.towerDeselectEvent.Invoke();
        OnTowerDeselect?.Invoke();
    }

    public void MoneySpent(int amountSpent)
    {
        // Enemies finish spawning + died / get to end
        Debug.Log($"Player Used Money: {amountSpent}");
        //current.moneySpentEvent.Invoke(amountSpent);
        OnMoneySpent?.Invoke(amountSpent);
    }
    public void TowerBlueprint(TD_Building tD_Building)
    {
        // Enemies finish spawning + died / get to end
        Debug.Log($"Building Blueprinting: " + tD_Building.name);
        //current.towerDeselectEvent.Invoke();
        OnTowerBlueprint?.Invoke(tD_Building);
    }

    public void TowerPlaced(TD_Building tD_Building)
    {
        // Enemies finish spawning + died / get to end
        Debug.Log($"Building Purchase: ");
        //current.towerDeselectEvent.Invoke();
        OnTowerPlace?.Invoke(tD_Building);
    }

    public void GenericCancel()
    {
        CustomEventArgs args = new CustomEventArgs();
        Debug.Log($"Cancel: ");
        OnCancel?.Invoke(args);
    }

    public void GenericAccept()
    {
        CustomEventArgs args = new CustomEventArgs();
        Debug.Log($"Accept: ");
        OnAccept?.Invoke(args);
    }

    public void Lose()
    {
        GameOver?.Invoke(this, EventArgs.Empty);
    }

    public void Win()
    {
        GameWon?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnEventHandled()
    {
        
    }

    public void ToggleGodMode()
    {
        OnGodModeToggle?.Invoke(this, EventArgs.Empty);
    }
}
