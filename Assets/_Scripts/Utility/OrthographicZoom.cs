using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class OrthographicZoom : MonoBehaviour
{
    private Vector3 homePosition;
    private Vector3 moveTarget;
    TD_Controls tdControls;
    PlayerInput playerInput;
    public Camera cam;
    public float maxZoom = 5;
    public float minZoom = 20;
    public float sensitivity = 1;
    public float speed = 30;
    float targetZoom;
    Vector2 moveVector;
    InputAction moveAction;

    public float movementSpeed = 25;
    private bool shouldMove;

    // Start is called before the first frame update
    void Start()
    {
        if (!cam) cam = Camera.main;
        homePosition = cam.transform.position;
        targetZoom = cam.orthographicSize;
        moveVector = Vector2.zero;
        moveTarget = Vector3.zero;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        UIControlsManager.PlayerMove += PlayerControlsManager_PlayerMove;
        PlayerControlsManager.PlayerMove += PlayerControlsManager_PlayerMove;
        PlayerControlsManager.GoHome += ReturnHome;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        cam = Camera.main;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UIControlsManager.PlayerMove -= PlayerControlsManager_PlayerMove;
        PlayerControlsManager.PlayerMove -= PlayerControlsManager_PlayerMove;
        PlayerControlsManager.GoHome -= ReturnHome;
    }

    /// <summary>
    /// Will receieve same event for start/stop of movement. Will pass different values in the movement field for start/stop
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="pInputArgs"></param>
    private void PlayerControlsManager_PlayerMove(object sender, PlayerInputEventArgs pInputArgs)
    {
        Debug.Log("PlayerMove" + pInputArgs.movement);
        MoveCamera(pInputArgs.movement);
    }

    // Update is called once per frame
    void Update()
    {
        AutoMove();
        AdjustZoom();
    }

    private void MoveCamera(Vector2 moveValue)
    {
        shouldMove = (moveValue.magnitude != 0);
        if (moveValue == Vector2.zero && moveVector == Vector2.zero) return;
        moveVector = moveValue;
        ThrottledMove(new Vector3(moveValue.x, moveValue.y, 0));
        //Vector3 movementAmount = new Vector3(moveValue.x, moveValue.y, 0) * movementSpeed * Time.deltaTime;
        //cam.transform.Translate(movementAmount);
        // This gets updated AFTER movement, so we dont update next frame.
    }

    private void AutoMove()
    {
        // TODO: Fix the return home functionality;
        //if (moveTarget != Vector3.zero)
        //{
        //    if (Vector3.Distance(cam.transform.position, moveTarget) > 2f)
        //    {
        //        ThrottledMove(Vector3.MoveTowards(moveTarget, cam.transform.position, 1f * Time.deltaTime).normalized);
        //    }
        //    else moveTarget = Vector3.zero;
        //}
        //else if (shouldMove)
        //{
        //    MoveCamera(moveVector);
        //}
        MoveCamera(moveVector);
    }

    private void AdjustZoom()
    {
        if (!cam) cam = Camera.main;
        else
        {
            targetZoom -= Mouse.current.scroll.ReadValue().normalized.y * sensitivity;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            float newSize = Mathf.MoveTowards(cam.orthographicSize, targetZoom, speed * Time.deltaTime);
            cam.orthographicSize = newSize;
        }
    }

    private void LateUpdate()
    {
        //Debug.Log("MOUSE ENABLED?" + UnityEngine.InputSystem.Mouse.current.enabled);

    }

    protected void ReturnHome(object sender, PlayerInputEventArgs pInputArgs)
    {
        moveTarget = homePosition;
        //moveVector = cam.transform.position - homePosition;
        //moveVector.Normalize();
    }

    private void ThrottledMove(Vector3 toMove)
    {
        Vector3 movementAmount = toMove * movementSpeed * Time.deltaTime;
        cam.transform.Translate(movementAmount);
    }
}
