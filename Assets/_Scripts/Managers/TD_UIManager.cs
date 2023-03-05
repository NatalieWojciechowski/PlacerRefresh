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
    public GameObject WaveStart;
    public GameObject playerMoney;
    public GameObject mainMenuPanel;
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
        WaveStart.GetComponent<Button>().onClick.AddListener(delegate { EventManager.instance.WaveStarted(TD_GameManager.instance.CurrentWaveIndex); });
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
        WaveStart.GetComponent<Button>().onClick.RemoveAllListeners();
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
            int currentWave = TD_GameManager.instance.CurrentWaveIndex + 1;
            if (!TD_EnemyManager.instance.WaveActive) currentWave--;
            waveStatus.GetComponentsInChildren<TMP_Text>()[1].text = $"{currentWave} / {TD_GameManager.instance.TotalWaves}";
        }
        if (playerMoney) playerMoney.GetComponentsInChildren<TMP_Text>()[0].text = TD_GameManager.instance.CurrentCurrency.ToString();
        if (WaveStart)
        {
            // If game over or 
            bool showStartButton = (
                TD_GameManager.instance.CoreHealth > 0 &&
                (!TD_EnemyManager.instance.WaveActive ||
                TD_GameManager.instance.IsWaitingForStart)
            );
            WaveStart.SetActive(showStartButton);
        }

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


    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    Debug.Log(eventData.selectedObject);
    //}

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    Debug.Log(eventData.selectedObject);
    //}

    //public void UpgradeSelectedTower()
    //{
    //    Debug.Log("UPGRADE", FindObjectOfType<TowerTooltip>());
    //}

    //public void SellSelectedTower()
    //{
    //    Debug.Log("Sell", FindObjectOfType<TowerTooltip>());
    //}

    //private void adjustBuildButtons()
    //{
    //    Button[] buildButtons = towerToolbar.gameObject.GetComponentsInChildren<Button>();
    //    if (buildButtons.Length == 0 || buildButtons.Length != TD_BuildManager.instance.Pieces.Count) return;
        
    //    for (int i = 0; i < TD_BuildManager.instance.Pieces.Count; i++)
    //    {
    //        if (TD_BuildManager.instance.Pieces[i] == null || buildButtons[i] == null) continue;

    //        Button currentButton = buildButtons[i];
    //        TD_Building buildingCtrl;
    //        TD_BuildManager.instance.Pieces[i].gameObject.TryGetComponent<TD_Building>(out buildingCtrl);
    //        if (buildingCtrl && buildingCtrl.GetStats().RawBuildingData)
    //        {
    //            currentButton.image.color = Color.white;
    //            int bCost = buildingCtrl.GetStats().RawBuildingData.PurchaseCost;
    //            currentButton.enabled = TD_GameManager.instance.CanAfford(bCost);
    //        } else {
    //            currentButton.enabled = false;
    //            currentButton.image.color = Color.red;
    //        }
    //    }
    //}

    //private void OnUserSpend(int bCost)
    //{
    //    if (!gameObject.activeSelf) return;
    //    EventManager.current.MoneySpent(bCost);
    //    // Prevent the user from placing another after only purchasing once
    //    // TODO: This doesnt behave as we would expect; still doesnt prevent more building
    //    //BuildManager.Instance.StopAllCoroutines();
    //    UpdateDisplay();
    //}
    public void ToggleMenu()
    {
        bool menuOpen = mainMenuPanel.activeSelf;
        if (menuOpen) TD_GameManager.SetGameSpeed(TD_GameManager.GameSpeedOptions.NORMAL);
        else TD_GameManager.SetGameSpeed(TD_GameManager.GameSpeedOptions.PAUSE);
        mainMenuPanel.SetActive(!menuOpen);
        if (SaveAndExitButton) SaveAndExitButton.GetComponent<Button>().interactable = !TD_GameManager.instance.HasStarted || TD_EnemyManager.instance.IsCurrentWaveComplete();
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
        ReInit();
    }
}
