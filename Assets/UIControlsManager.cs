using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIControlsManager : MonoBehaviour, TD_Controls.IUIActions
{
    [SerializeField]
    public static TD_Controls controls;

    public static event EventHandler<PlayerInputEventArgs> PlayerMove;

    public void OnEnable()
    {
        if (controls == null) controls = new TD_Controls();
        //{
        //    controls = new TD_Controls();
        //    // Tell the "gameplay" action map that we want to get told about
        //    // when actions get triggered.
        //    controls.UI.SetCallbacks(this);
        //}
        //controls.TD_BuilderControls.Enable();
        //controls.UI.Enable();
    }


    public void OnAccept(InputAction.CallbackContext context)
    {
        Debug.Log("UI OnAccept" + context);
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        Debug.Log("UI OnCancel!" + context);
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        Debug.Log("UI OnClick!" + context);
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        Debug.Log("UI MiddleClick" + context);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("context.action.ReadValue<Vector2>()" + context.action.ReadValue<Vector2>().ToString());
        if (context.action.triggered && context.action.ReadValue<Vector2>().magnitude != 0 && context.action.phase == InputActionPhase.Performed)
        {
            //Perform Trigger Pressed Actions
            PlayerMove(this, new PlayerInputEventArgs(context.ReadValue<Vector2>().normalized));
        }
        else if (context.action.triggered && context.action.ReadValue<Vector2>().magnitude == 0 && context.action.phase == InputActionPhase.Performed)
        {
            //Perform Trigger Release Actions
            PlayerMove(this, new PlayerInputEventArgs(Vector2.zero));
        }


        Debug.Log("UI Onmove" + context);
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        Debug.Log("UI OnNavigate" + context);
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        //Debug.Log("UI OnPoint" + context);
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        Debug.Log("UI OnRightClick" + context);
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        Debug.Log("UI OnScrollWheel" + context);
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        Debug.Log("UI OnSubmit" + context);
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
        Debug.Log("UI OnTrackedDeviceOrientation" + context);
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
        Debug.Log("UI OnTrackedDevicePosition" + context);
    }

    // Start is called before the first frame update
    void Start()
    {
        //controls = GetComponent<PlayerInput>().control;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
