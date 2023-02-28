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

public class PlayerControlsManager : MonoBehaviour, TD_Controls.IPlayerActions
{
    // MyPlayerControls is the C# class that Unity generated.
    // It encapsulates the data from the .inputactions asset we created
    // and automatically looks up all the maps and actions for us.
    [SerializeField]
    public static TD_Controls controls;

    [SerializeField]
    private PlayerInput playerInput;

    public static TD_Controls currentMap;
    
    private TD_Controls.PlayerActions playerMap;
    private TD_Controls.TD_BuilderControlsActions builderMap;
    private TD_Controls.UIActions uiMap;

    public static event EventHandler PlayerCancel;
    public static event EventHandler PlayerAccept;
    public static event EventHandler<PlayerInputEventArgs> PlayerMove;
    public static event EventHandler<PlayerInputEventArgs> GoHome;

    public void OnEnable()
    {
        EnableAllControls();
        //playerMap = controls.Player;
        //builderMap = controls.TD_BuilderControls;
        //uiMap = controls.UI;
        //UIMode();
        //controls.TD_BuilderControls.Enable();
        //controls.Player.Enable();
    }

    private void EnableAllControls()
    {
        if (!playerInput) playerInput = FindObjectOfType<PlayerInput>();
        if (controls == null) controls = new();
        controls.Enable();
        controls.Player.Enable();
        controls.TD_BuilderControls.Enable();
        controls.UI.Enable();
        playerInput.enabled = true;
    }

    public void OnDisable()
    {
        //controls.TD_BuilderControls.Disable();    
        if (playerInput) playerInput.enabled = false;
    }

    public void OnMoveAlt(InputAction.CallbackContext context)
    {
        //Debug.Log("PCM => OnMove" + context.ToString());
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
        EnableAllControls();
        DontDestroyOnLoad(transform.gameObject);
        //if (controls == null) controls = new TD_Controls();
        //UIMode();
    }

    // Update is called once per frame
    void Update()
    {
        currentMap = controls;
    }

    private void BuilderMode()
    {
        //builderMap.Enable();
        //playerMap.Disable();
        //uiMap.Disable();
    }

    private void PlayerMode()
    {
        //builderMap.Disable();
        //playerMap.Enable();
        //uiMap.Disable();
    }

    private void UIMode()
    {
        //playerInput.SwitchCurrentControlScheme("");
        //builderMap.Disable();
        //playerMap.Disable();
        //uiMap.Enable();
    }

    public void OnAccept(InputAction.CallbackContext context)
    {
        //Debug.Log("Accept!" + context);
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue(), 0);
        //RaycastHit2D ray2D = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(mousePos));

        //if (ray2D)
        //{
        //    GameObject foundObject = ray2D.collider.gameObject;
        //    Debug.Log("HIT OBJECT: " + foundObject);
        //    // Do something else

        //}

        //RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit[] inWay = Physics.RaycastAll(ray, 20f);
        if (inWay.Length > 0)
        {

            foreach (RaycastHit hitTarget in inWay)
            {
                print(hitTarget.collider.name);
            }
        }
        //RaycastHit hit = Physics.Raycast(mousePos, Vector3.zero);
        //if (hit)
        //{
        //    // Call methods here
        //    Debug.Log("Raycast Hit -> " + hit.transform.name);
        //}

        EventManager.instance.GenericAccept();
        PlayerAccept(this, new PlayerInputEventArgs(Mouse.current.position.ReadValue()));
    }

    //public void OnCancel(InputAction.CallbackContext context)
    //{
    //    Debug.Log("Cancel!" + context);
    //    EventManager.current.GenericCancel();
    //    PlayerCancel(this, EventArgs.Empty);
    //}

    //public void OnLeftClick(InputAction.CallbackContext context)
    //{

    //    Debug.Log("OnLeftClick" + context);
    //    // TODO: Mode check?
    //    OnAccept(context);
    //}

    //public void OnMiddleClick(InputAction.CallbackContext context)
    //{
    //    Debug.Log("OnMiddleClick" + context);
    //    Debug.Log("Middle Click");
    //}

    //public void OnRightClick(InputAction.CallbackContext context)
    //{

    //    Debug.Log("OnRightClick" + context);
    //    // TODO: Mode check?
    //    OnCancel(context);
    //}

    public void OnFire(InputAction.CallbackContext context)
    {
        Debug.Log("OnFire");
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Debug.Log("OnLook");
    }

    public void OnHome(InputAction.CallbackContext context)
    {
        Debug.Log("SEND CAMERA HOME");
        GoHome(this, new PlayerInputEventArgs(Vector2.zero));
    }
}