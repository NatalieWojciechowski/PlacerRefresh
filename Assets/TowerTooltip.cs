using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerTooltip : MonoBehaviour
{
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

    private void Awake()
    {
        EventManager.OnTowerSelect += (bSelected) => SelectBuilding(bSelected);
    }

    private void OnDisable()
    {
        EventManager.OnTowerSelect -= (bSelected) => SelectBuilding(bSelected);
        upgradeButton.onClick.RemoveAllListeners();
        sellButton.onClick.RemoveAllListeners();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RefreshInfo();
    }

    private void SelectBuilding(TD_Building bSelected)
    {
        selectedBuilding = bSelected;
        if (selectedBuilding) TowerSelectionUI.SetActive(true);
        else TowerSelectionUI?.gameObject.SetActive(false);
    }

    private void RefreshInfo()
    {
        if (!TowerSelectionUI) return;
        if (!selectedBuilding)
        {
            ResetUI();
            return;
        }

        TD_BuildingData bStats = selectedBuilding.GetStats();

        towerNameText.text = bStats.displayName;

        // Setup Upgrade button
        if (bStats.upgradesTo == null) SetupButton(upgradeButton, AttemptUpgrade, selectedBuilding);
        else CleanupButton(upgradeButton, AttemptUpgrade, selectedBuilding);

        // Setup Sell button
        if (bStats.canSell) SetupButton(sellButton, AttemptSell, selectedBuilding);
        else CleanupButton(sellButton, AttemptSell, selectedBuilding);

        // Update enemies killed info
        // TODO: dunno if I want to keep this
        enemiesKilledText.text = selectedBuilding.EnemyKillCount.ToString();

        // update dmg numbers and type
        dmgAmtText.text = bStats.baseDamage.ToString();
        dmgTypeText.text = "Normal";
    }

    private void SetupButton(Button tButton, Action<TD_Building> bCallback, TD_Building tD_Building)
    {
        upgradeButton.interactable = true;
        tButton.onClick.AddListener(() => bCallback(tD_Building));
    }

    private void CleanupButton(Button tButton, Action<TD_Building> bCallback, TD_Building tD_Building)
    {
        upgradeButton.interactable = false;
        tButton.onClick.RemoveListener(() => bCallback(tD_Building));
    }

    private void AttemptUpgrade(TD_Building selectedTower)
    {
        Debug.Log("Try upgrade building", selectedTower);
        selectedTower.TrySell();
    }
    private void AttemptSell(TD_Building selectedTower)
    {
        Debug.Log("Try SELL building", selectedTower);
        selectedTower.TrySell();
    }

    private void ResetUI()
    {
        // TODO: make sure that the callbacks are disabled at this point 
        TowerSelectionUI.gameObject.SetActive(false);
    }
}
