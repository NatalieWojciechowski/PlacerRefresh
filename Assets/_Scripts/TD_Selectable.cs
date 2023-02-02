using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TD_Selectable : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        TD_Building tD_Building = GetComponent<TD_Building>();
        TD_Enemy tD_Enemy = GetComponent<TD_Enemy>();

        if (tD_Building) handleBuilding(tD_Building);
        else if (tD_Enemy) handleEnemy(tD_Enemy);
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
