using EasyBuildSystem.Features.Scripts.Core.Base.Builder;
using EasyBuildSystem.Features.Scripts.Core.Base.Manager;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TD_UIManager : MonoBehaviour, I_RefreshOnSceneChange
{
    public static TD_UIManager instance;
    [SerializeField] TD_GameSerializer GameSerializer;

    public GameObject coreStatus;
    public GameObject gameOverStatus;
    public GameObject gameWinStatus;
    public GameObject waveStatus;
    public GameObject SpeedControls;
    public GameObject WaveStartButton;
    public GameObject playerMoney;
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject SaveAndExitButton;
    public GameObject HealthBarContainer;

    public ModelShark.TooltipStyle tooltipStyle;

    //public Button ButtonTemplate;
    //public Transform Container;


    //TD_Controls td_controls;
    private void Awake()
    {
        //td_controls  = new TD_Controls();
        //td_controls.TD_BuilderControls.SetCallbacks()
        //td_controls.TD_BuilderControls.Accept.performed += (() => EventManager.current.TowerDeselected());
        UpdateDisplay();
    }


    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChange;
        SceneManager.sceneLoaded += OnSceneLoad;
        EventManager.OnTowerSelect += OnTowerSelect;
        EventManager.OnTowerDeselect += UpdateDisplay;
        EventManager.OnWaveStart += OnWaveStart;
        EventManager.GameOver += OnGameLose;
        EventManager.GameWon += OnGameWin;
        WaveStartButton.GetComponent<Button>().onClick.AddListener(OnPlayerStart);
        //waveStatus.GetComponentInChildren<Button>().onClick.AddListener(delegate { EventManager.current.WaveStarted(TD_GameManager.current.CurrentWaveIndex); });
        Button[] speedButtons = SpeedControls.GetComponentsInChildren<Button>();
        speedButtons[0]?.onClick.AddListener(() => TD_GameManager.SetGameSpeed(TD_GameManager.GameSpeedOptions.PAUSE));
        speedButtons[1]?.onClick.AddListener(() => TD_GameManager.SetGameSpeed(TD_GameManager.GameSpeedOptions.NORMAL));
        speedButtons[2]?.onClick.AddListener(() => TD_GameManager.SetGameSpeed(TD_GameManager.GameSpeedOptions.FAST));
        speedButtons[3]?.onClick.AddListener(() => TD_GameManager.SetGameSpeed(TD_GameManager.GameSpeedOptions.FASTER));
        speedButtons[4]?.onClick.AddListener(() => TD_GameManager.SetGameSpeed(TD_GameManager.GameSpeedOptions.FASTEST));
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
        SceneManager.sceneLoaded -= OnSceneLoad;
        EventManager.OnTowerSelect -= OnTowerSelect;
        EventManager.OnTowerDeselect -= UpdateDisplay;
        EventManager.GameOver -= OnGameLose;
        EventManager.GameWon -= OnGameWin;
        EventManager.OnWaveStart -= OnWaveStart;
        //waveStatus.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
        WaveStartButton.GetComponent<Button>().onClick.RemoveAllListeners();
        Button[] speedButtons = SpeedControls.GetComponentsInChildren<Button>();
        foreach (Button button in speedButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else Destroy(gameObject);
    }

    private void OnPlayerStart()
    {
        if (EventManager.instance == null) return;
        EventManager.instance.PlayerReady();
        UpdateDisplay();
    }

    private void OnTowerSelect(TD_Building selectedBuilding)
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (!TD_GameManager.instance || !TD_EnemyManager.instance) return;
        if (coreStatus) coreStatus.GetComponentInChildren<TMP_Text>().text = TD_GameManager.instance.CoreHealth.ToString();

        if (waveStatus)
        {
            int currentWave = TD_GameManager.instance.CurrentWaveIndex;
            if (TD_EnemyManager.instance.SpawnersActive || TD_GameManager.instance.IsWaitingForStart) currentWave += 1;
            //if (!TD_EnemyManager.instance.WaveActive) currentWave--;
            waveStatus.GetComponentsInChildren<TMP_Text>()[1].text = $"{currentWave} / {TD_GameManager.instance.TotalWaves}";
        }
        if (playerMoney) playerMoney.GetComponentsInChildren<TMP_Text>()[0].text = TD_GameManager.instance.CurrentCurrency.ToString();

        if (WaveStartButton)
        {
            // Dont allow early wave start if midwave already
            bool showStartButton = (
                TD_GameManager.instance.CoreHealth > 0 &&
                (!TD_EnemyManager.instance.AutoStart && TD_GameManager.instance.IsWaitingForStart)
            );
            WaveStartButton.SetActive(showStartButton);
        }

        // TODO: Can this just be !showStartButton ?
        // We only want to be able to save if not mid-wave
        if (SaveAndExitButton) SaveAndExitButton.GetComponent<Button>().interactable = !TD_GameManager.instance.PlayerReady || TD_EnemyManager.instance.IsCurrentWaveGroupComplete();

        // We may have added new buildings or something cannot be afforded anymore
        if (TD_BuildManager.instance)
        {
            TD_BuildManager.instance.UpdateBuildToolbar();
        }
    }

    private void UpdateForPrices()
    {

    }

    private void OnGameLose(object sender, EventArgs e)
    {
        gameOverStatus?.SetActive(true);
    }

    private void OnGameWin(object sender, EventArgs e)
    {
        gameWinStatus?.SetActive(true);
    }

    private void OnWaveStart(int waveId)
    {
        UpdateDisplay();
    }

    private void Update()
    {
        UpdateDisplay();
    }

    public void ToggleMenu()
    {
        bool alreadyOpen = mainMenuPanel.activeSelf;
        if (alreadyOpen)
        {
            // Unpause
            TD_GameManager.SetGameSpeed(TD_GameManager.GameSpeedOptions.NORMAL);
        }
        else
        {
            TD_GameManager.SetGameSpeed(TD_GameManager.GameSpeedOptions.PAUSE);
            if (settingsPanel) settingsPanel.SetActive(false);
        }
        mainMenuPanel.SetActive(!alreadyOpen);
    }


    public void ToggleSettings()
    {
        if (settingsPanel == null) return;
        // Do we ALREADY have the panel open? 
        bool settingsOpen = settingsPanel.activeSelf;
        // Game should already be paused from the toggle menu at this point; we also dont want to unpause yet
        settingsPanel.SetActive(!settingsOpen);
    }

    public void ApplySettings()
    {
        // AUDIO manager has made the actual updates; without this we will discard changes
        PlayerPrefs.Save();
        ToggleSettings();
    }

    public void SaveAndExit()
    {
        TD_GameSerializer.SaveGame();
        FindObjectOfType<SceneLoader>().SetNextScene(SceneLoader.GameScene.MainMenu);
    }

    public void OnSceneChange(Scene current, Scene next)
    {

    }

    public void ReInit()
    {
        this.UpdateDisplay();

        // TODO: This might need to also init the click handlers on non-menu levels
    }

    public void OnSceneLoad(Scene current, LoadSceneMode loadSceneMode)
    {
        if (mainMenuPanel.activeSelf) ToggleMenu();
        if (settingsPanel && settingsPanel.activeSelf) ToggleMenu();
        ReInit();
    }
}
