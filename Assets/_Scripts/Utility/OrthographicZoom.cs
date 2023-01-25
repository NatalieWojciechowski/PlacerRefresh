using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrthographicZoom : MonoBehaviour
{
    private Vector3 homePosition;
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
        homePosition = transform.position;
        targetZoom = cam.orthographicSize;
        moveVector = Vector2.zero;
    }

    private void OnEnable()
    {
        PlayerControlsManager.PlayerMove += PlayerControlsManager_PlayerMove;
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

    private void OnDisable()
    {
        PlayerControlsManager.PlayerMove -= PlayerControlsManager_PlayerMove;
    }

    private void MoveCamera(Vector2 moveValue)
    {
        shouldMove = (moveValue.magnitude != 0);
        if (moveValue == Vector2.zero && moveVector == Vector2.zero) return;
        moveVector = moveValue;
        Vector3 movementAmount = new Vector3(moveValue.x, moveValue.y, 0) * movementSpeed * Time.deltaTime;
        cam.transform.Translate(movementAmount);
        // This gets updated AFTER movement, so we dont update next frame.
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldMove)
        {
            MoveCamera(moveVector);
        }
        targetZoom -= Mouse.current.scroll.y.ReadValue() * sensitivity;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        float newSize = Mathf.MoveTowards(cam.orthographicSize, targetZoom, speed * Time.deltaTime);
        cam.orthographicSize = newSize;
    }

    private void LateUpdate()
    {
        //Debug.Log("MOUSE ENABLED?" + UnityEngine.InputSystem.Mouse.current.enabled);

    }
}
