using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDGodMode : MonoBehaviour
{
    [SerializeField] private bool isEnabled;

    // Start is called before the first frame update
    void Start()
    {

//#if DEBUG
//        isEnabled = true;
//#endif
        // So we dont have to add a bunch of guardrails below. 
        if (!isEnabled) gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.OnGodModeToggle += EventManager_OnGodModeToggle;
    }

    private void OnDisable()
    {
        EventManager.OnGodModeToggle -= EventManager_OnGodModeToggle;
    }

    private void EventManager_OnGodModeToggle(object sender, EventArgs e)
    {
        Debug.Log("Toggle God Mode FROM" + isEnabled);
        isEnabled = !isEnabled;
        Debug.Log("Toggle God Mode TO" + isEnabled);

        if (isEnabled) TD_GameManager.instance.AddCoins(100);
    }

    // Update is called once per frame
    void Update()
    {
        // Trigger Event in Game Manager? 
        if (!TD_GameManager.instance.CanAfford(100)) TD_GameManager.instance.AddCoins(100);
    }
}
