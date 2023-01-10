using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TD_UIManager : MonoBehaviour
{
    public GameObject coreStatus;
    public GameObject gameOverStatus;
    public GameObject waveStatus;
    public GameObject SpeedControls;

    private void Awake()
    {
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
        waveStatus.GetComponentInChildren<Button>().onClick.RemoveListener(() => EventManager.WaveStarted(TD_GameManager.current.CurrentWave));
        Button[] speedButtons = SpeedControls.GetComponentsInChildren<Button>();
        foreach (Button button in speedButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    private void Start()
    {
        
    }
    public void UpdateDisplay()
    {
        if (!TD_GameManager.current) return;
        if (coreStatus) coreStatus.GetComponentInChildren<TMP_Text>().text = TD_GameManager.current.CoreHealth.ToString();
        if (TD_GameManager.current.CoreHealth <= 0) gameOverStatus.SetActive(true);
        if (waveStatus) waveStatus.GetComponentsInChildren<TMP_Text>()[1].text = TD_GameManager.current.CurrentWave.ToString();

        // TODO: Outline the current speed
    }
}
