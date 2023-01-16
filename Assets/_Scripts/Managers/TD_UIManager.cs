using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TD_UIManager : MonoBehaviour
{
    public static TD_UIManager current;

    public GameObject coreStatus;
    public GameObject gameOverStatus;
    public GameObject waveStatus;
    public GameObject SpeedControls;
    public GameObject playerMoney;

    //TD_Controls td_controls;
    private void Awake()
    {
        //td_controls  = new TD_Controls();
        //td_controls.TD_BuilderControls.SetCallbacks()
        //td_controls.TD_BuilderControls.Accept.performed += (() => EventManager.current.TowerDeselected());
        waveStatus.GetComponentInChildren<Button>().onClick.AddListener(() => EventManager.WaveStarted(TD_GameManager.current.CurrentWave));
        Button[] speedButtons = SpeedControls.GetComponentsInChildren<Button>();
        speedButtons[0]?.onClick.AddListener(() => TD_GameManager.SetGameSpeed(TD_GameManager.GameSpeedOptions.PAUSE));
        speedButtons[1]?.onClick.AddListener(() => TD_GameManager.SetGameSpeed(TD_GameManager.GameSpeedOptions.NORMAL));
        speedButtons[2]?.onClick.AddListener(() => TD_GameManager.SetGameSpeed(TD_GameManager.GameSpeedOptions.FAST));
        speedButtons[3]?.onClick.AddListener(() => TD_GameManager.SetGameSpeed(TD_GameManager.GameSpeedOptions.FASTER));

        UpdateDisplay();
    }

    private void OnDisable()
    {
        EventManager.OnTowerDeselect -= () => UpdateDisplay();
        waveStatus.GetComponentInChildren<Button>().onClick.RemoveListener(() => EventManager.WaveStarted(TD_GameManager.current.CurrentWave));
        Button[] speedButtons = SpeedControls.GetComponentsInChildren<Button>();
        foreach (Button button in speedButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    private void Start()
    {
        if (current != null) Destroy(this);
        current = this;
    }
    public void UpdateDisplay()
    {
        if (!TD_GameManager.current) return;
        if (coreStatus) coreStatus.GetComponentInChildren<TMP_Text>().text = TD_GameManager.current.CoreHealth.ToString();
        if (TD_GameManager.current.CoreHealth <= 0) gameOverStatus.SetActive(true);
        if (waveStatus) waveStatus.GetComponentsInChildren<TMP_Text>()[1].text = TD_GameManager.current.CurrentWave.ToString();
        if (playerMoney) playerMoney.GetComponentsInChildren<TMP_Text>()[0].text = TD_GameManager.current.CurrentCurrency.ToString();
        // TODO: Outline the current speed
        
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

}
