using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuilderControlsManager : MonoBehaviour
{
    public static TD_Controls controls;
    public static event EventHandler<PlayerInputEventArgs> CameraMove;

    public void OnEnable()
    {
        if (controls == null)
        {
            controls = new TD_Controls();
            // Tell the "gameplay" action map that we want to get told about
            // when actions get triggered.
            //controls.TD_BuilderControls.SetCallbacks(this);
        }
        //controls.TD_BuilderControls.Enable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {        
        //Debug.Log("PCM => OnMove" + context.ToString());
        Debug.Log("context.action.ReadValue<Vector2>()" + context.action.ReadValue<Vector2>().ToString());
        if (context.action.triggered && context.action.ReadValue<Vector2>().magnitude != 0 && context.action.phase == InputActionPhase.Performed)
        {
            //Perform Trigger Pressed Actions
            CameraMove(this, new PlayerInputEventArgs(context.ReadValue<Vector2>().normalized));
        }
        else if (context.action.triggered && context.action.ReadValue<Vector2>().magnitude == 0 && context.action.phase == InputActionPhase.Performed)
        {
            //Perform Trigger Release Actions
            CameraMove(this, new PlayerInputEventArgs(Vector2.zero));
        }
    }

    public void OnAccept(InputAction.CallbackContext context)
    {
        Debug.Log("BCM, OnAccept!" + context);
    }

    public void OnCancelAlt(InputAction.CallbackContext context)
    {
        Debug.Log("BCM, OnCancel!" + context);
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
