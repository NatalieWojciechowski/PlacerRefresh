using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerControlsManager : MonoBehaviour, TD_Controls.ITD_BuilderControlsActions
{

    // MyPlayerControls is the C# class that Unity generated.
    // It encapsulates the data from the .inputactions asset we created
    // and automatically looks up all the maps and actions for us.
    TD_Controls controls;

    public void OnEnable()
    {
        if (controls == null)
        {
            controls = new TD_Controls();
            // Tell the "gameplay" action map that we want to get told about
            // when actions get triggered.
            controls.TD_BuilderControls.SetCallbacks(this);
        }
        controls.TD_BuilderControls.Enable();
    }

    public void OnDisable()
    {
        controls.TD_BuilderControls.Disable();
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        // 'Use' code here.
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // 'Move' code here.
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnAccept(InputAction.CallbackContext context)
    {
        Debug.Log("Accept!" + context);
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        Debug.Log("Cancel!" + context);
        EventManager.TowerDeselected();
    }
}