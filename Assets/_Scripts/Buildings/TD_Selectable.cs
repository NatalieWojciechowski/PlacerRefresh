using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TD_Selectable : MonoBehaviour, IPointerClickHandler
{
    private bool isSelected = false;
    public bool IsSelected { get => isSelected; }

    public void OnPointerClick(PointerEventData eventData)
    {
        isSelected = true;

        TD_Building tD_Building = GetComponent<TD_Building>();
        TD_Enemy tD_Enemy = GetComponent<TD_Enemy>();

        if (tD_Building) OnBuildingSelected(tD_Building);
        else if (tD_Enemy) handleEnemy(tD_Enemy);
    }

    public void OnBuildingSelected(TD_Building tD_Building)
    {
        handleBuilding(tD_Building);
        TD_SelectionManager.instance.SetSelected(this);
    }

    /// <summary>
    /// From the SelectionManager to the selectable
    /// </summary>
    public void OnBuildingDeselected()
    {
        // 
    }

    private void handleEnemy(TD_Enemy selectedEnemy)
    {
        Debug.Log("ENEMY SELECTED");
    }

    private void handleBuilding(TD_Building selectedBuilding)
    {
        // Dont select the building we just placed
        // TODO: perhaps just have a "build time" for the building before collider active?
        if (TD_BuildManager.current.GetBuildState() == TD_BuildManager.BuildState.Cooldown) return;

        if (selectedBuilding.IsRunning) EventManager.current.TowerSelected(selectedBuilding);
    }

    // Start is called before the first frame update
    void Start()
    {
        TD_SelectionManager.instance.Selectables.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        TD_SelectionManager.instance.Selectables.Remove(this);
    }

}
