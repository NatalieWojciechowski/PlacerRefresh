using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputEventArgs : EventArgs
{
    public Vector2 movement;
    public PlayerInputEventArgs(Vector2 _movement)
    {
        movement = _movement;
    }
}


public class PlayerControlsManager : MonoBehaviour, TD_Controls.ITD_BuilderControlsActions
{
    // MyPlayerControls is the C# class that Unity generated.
    // It encapsulates the data from the .inputactions asset we created
    // and automatically looks up all the maps and actions for us.
    TD_Controls controls;

    public static event EventHandler PlayerCancel;
    public static event EventHandler PlayerAccept;
    public static event EventHandler<PlayerInputEventArgs> PlayerMove;

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
        Debug.Log("PCM => OnMove" + context.ToString());
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

        //Vector2 movementInput;
        //float smoothTime = 0.3f;
        //float hVelocity = 0f;
        //float vVecolity = 0f;
        //float hCurrent = 0f;
        //float vCurrent = 0f;


        //// 'Move' code here.
        //_controls.Player.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>().normalized;

        //float newH = Mathf.SmoothDamp(hCurrent, movementInput.x, ref hVelocity, smoothTime);
        //float newV = Mathf.SmoothDamp(vCurrent, movementInput.y, ref vVecolity, smoothTime);
        //hCurrent = newH;
        //vCurrent = newV;
        PlayerMove(this, new PlayerInputEventArgs(context.ReadValue<Vector2>().normalized));
        //PlayerMove(this, new Vector2(Mouse.current.delta.x.value, Mouse.current.delta.y));
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
        EventManager.current.GenericAccept();
        PlayerAccept(this, EventArgs.Empty);
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        Debug.Log("Cancel!" + context);
        EventManager.current.GenericCancel();
        PlayerCancel(this, EventArgs.Empty);
    }

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        // TODO: Mode check?
        OnAccept(context);
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        Debug.Log("Middle Click");
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        // TODO: Mode check?
        OnCancel(context);
    }
}