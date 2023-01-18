using EasyBuildSystem.Features.Scripts.Core.Base.Builder;
using EasyBuildSystem.Features.Scripts.Core.Base.Manager;
using System;
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

    public GameObject pieces_Selection;

    //public Button ButtonTemplate;
    //public Transform Container;


    //TD_Controls td_controls;
    private void Awake()
    {
        //td_controls  = new TD_Controls();
        //td_controls.TD_BuilderControls.SetCallbacks()
        //td_controls.TD_BuilderControls.Accept.performed += (() => EventManager.current.TowerDeselected());
        EventManager.OnTowerDeselect += () => UpdateDisplay();
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
        if (pieces_Selection)
        {
            adjustBuildButtons();
        }
    }

    private void UpdateForPrices()
    {

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

    private void adjustBuildButtons()
    {
        Button[] buildButtons = pieces_Selection.gameObject.GetComponentsInChildren<Button>();
        if (buildButtons.Length != BuildManager.Instance.Pieces.Count) Debug.Break();

        for (int i = 0; i < BuildManager.Instance.Pieces.Count; i++)
        {
            if (BuildManager.Instance.Pieces[i] == null || buildButtons[i] == null) continue;

            Button currentButton = buildButtons[i];
            TD_Building buildingCtrl;
            BuildManager.Instance.Pieces[i].gameObject.TryGetComponent<TD_Building>(out buildingCtrl);
            if (buildingCtrl && buildingCtrl.GetStats().RawBuildingData)
            {
                currentButton.image.color = Color.white;
                int bCost = buildingCtrl.GetStats().RawBuildingData.PurchaseCost;
                currentButton.enabled = TD_GameManager.current.CanAfford(bCost);
                currentButton.onClick.AddListener(() => OnUserSpend(bCost));
            } else {
                currentButton.enabled = false;
                //currentButton.image.color = new Color(1f, 0, 0, 0.25f);
                currentButton.onClick.RemoveListener(() => OnUserSpend(0));
            }

            //int Index = i;
            //currentButton.onClick.AddListener(() =>
            //{
            //    BuilderBehaviour.Instance.ChangeMode(EasyBuildSystem.Features.Scripts.Core.Base.Builder.Enums.BuildMode.None);
            //    BuilderBehaviour.Instance.SelectPrefab(BuildManager.Instance.Pieces[Index]);
            //    BuilderBehaviour.Instance.ChangeMode(EasyBuildSystem.Features.Scripts.Core.Base.Builder.Enums.BuildMode.Placement);
            //});

            //Button.transform.GetChild(0).GetComponent<Image>().sprite = BuildManager.Instance.Pieces[i].Icon;
            //Button.transform.GetChild(0).GetComponent<Image>().preserveAspect = true;

            //Button.transform.GetChild(1).GetComponent<Text>().text = BuildManager.Instance.Pieces[i].Name;
        }
    }

    private void OnUserSpend(int bCost)
    {
        EventManager.MoneySpent(bCost);
        // Prevent the user from placing another after only purchasing once
        // TODO: This doesnt behave as we would expect; still doesnt prevent more building
        BuildManager.Instance.StopAllCoroutines();
        UpdateDisplay();
    }
}
