using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TowerTooltip : MonoBehaviour
{
    public static TowerTooltip current;

    private TD_Building selectedBuilding;
    [SerializeField]
    private GameObject TowerSelectionUI;
    [SerializeField]
    private Button upgradeButton;
    [SerializeField]
    private Button sellButton;
    [SerializeField]
    private TMP_Text towerNameText;
    [SerializeField]
    private TMP_Text dmgAmtText;
    [SerializeField]
    private TMP_Text dmgTypeText;
    [SerializeField]
    private TMP_Text enemiesKilledText;

    private float lastAction = 0f;
    private float _cooldownPeriod = 1.25f;

    private void OnEnable()
    {
        EventManager.OnTowerSelect += SelectBuilding;
        EventManager.OnTowerDeselect += DeselectBuilding;
    }

    private void DeselectBuilding()
    {
        SelectBuilding(null);
    }

    private void OnDisable()
    {
        EventManager.OnTowerSelect -= SelectBuilding;
        EventManager.OnTowerDeselect -= DeselectBuilding;
        upgradeButton.onClick.RemoveAllListeners();
        sellButton.onClick.RemoveAllListeners();
    }


    // Start is called before the first frame update
    void Start()
    {
        if (current != null) Destroy(this);
        current = this;
        ResetUI();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Update display for enemies killed count, rest can be done post interactions
    }

    private void SelectBuilding(TD_Building bSelected)
    {
        if (bSelected)
        {
            // Avoid setting up listeners if we already have the building selected
            if (!selectedBuilding || bSelected.BuildingUUID != selectedBuilding.BuildingUUID)
            {
                selectedBuilding = bSelected;
                TowerSelectionUI.SetActive(true);

                BuildingData bStats = selectedBuilding.GetStats();
                //// Setup Upgrade button
                //upgradeButton.OnPointerClick(HandleUpgrade());
                if (bStats.UpgradesTo == null) upgradeButton.onClick.AddListener(() => AttemptUpgrade());
                else upgradeButton.onClick.RemoveListener(() => AttemptUpgrade());

                // Setup Sell button
                if (bStats.CanSell) sellButton.onClick.AddListener(() => AttemptSell());
                else sellButton.onClick.RemoveListener(() => AttemptSell());
            }
        }
        else
        {
            selectedBuilding = null;
            TowerSelectionUI?.gameObject.SetActive(false);
        }
        RefreshInfo();
    }

    private void RefreshInfo()
    {
        if (!selectedBuilding)
        {
            ResetUI();
            return;
        }
        //if (!TowerSelectionUI || !towerNameText) return;
        BuildingData bStats = selectedBuilding.GetStats();
        towerNameText.text = bStats.DisplayName;

        // Update enemies killed info
        // TODO: dunno if I want to keep this
        enemiesKilledText.text = selectedBuilding.EnemyKillCount.ToString();

        // update dmg numbers and type
        dmgAmtText.text = bStats.Damage.ToString();
        dmgTypeText.text = "Normal";
    }    

    private void HandleUpgrade(PointerEventData pointerEventData)
    {
        if (pointerEventData.fullyExited) {
            Debug.Log(pointerEventData);
        }
    }

    public void AttemptUpgrade()
    {
        if (!CooldownMet() || !TowerSelectionUI.activeInHierarchy || !TowerSelectionUI.scene.IsValid()) return;
        Debug.Log("Try upgrade building", selectedBuilding);
        SetActionTimeOnSuccess(selectedBuilding.TryUpgrade());
    }
    public void AttemptSell()
    {
        if (!CooldownMet() || !TowerSelectionUI.activeInHierarchy || !TowerSelectionUI.scene.IsValid()) return;
        Debug.Log("Try SELL building", selectedBuilding);
        SetActionTimeOnSuccess(selectedBuilding.TrySell());
    }

    // TODO: Ideally this should be only one managing the last action time
    private void SetActionTimeOnSuccess(bool actionResult)
    {
        if (actionResult) lastAction = Time.time;
        RefreshInfo();
    }

    private bool CooldownMet()
    {
        bool timeExpired = (Time.time - lastAction > _cooldownPeriod);
        // Update for other attempts within this
        lastAction = Time.time;
        return timeExpired;
    }

    private void ResetUI()
    {
        // TODO: make sure that the callbacks are disabled at this point 
        upgradeButton.onClick.RemoveAllListeners();
        sellButton.onClick.RemoveAllListeners();
        TowerSelectionUI?.gameObject.SetActive(false);
    }

    private void Deselect()
    {
        selectedBuilding = null;
        ResetUI();
    }

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    if (eventData.selectedObject == upgradeButton && upgradeButton.IsActive())
    //    {
    //        AttemptUpgrade();
    //    } else if (eventData.selectedObject == sellButton && sellButton.IsActive())
    //    {
    //        AttemptSell(selectedBuilding);
    //    }
    //}

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    //((IPointerClickHandler)sellButton).OnPointerClick(eventData);
    //    //((IPointerClickHandler)upgradeButton).OnPointerClick(eventData);
    //    //{
    //    //    AttemptUpgrade(selectedBuilding);
    //    //} else if (eventData.selectedObject == sellButton && sellButton.IsActive())
    //    //{
    //    //    AttemptSell(selectedBuilding);
    //    //}
    //    if (eventData.selectedObject == upgradeButton && upgradeButton.IsActive()) { 
    //        AttemptUpgrade(selectedBuilding);
    //    }
    //    else if (eventData.selectedObject == sellButton && sellButton.IsActive())
    //    {
    //        AttemptSell(selectedBuilding);
    //    }
    //}
}
