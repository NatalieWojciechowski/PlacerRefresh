using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrthographicZoom : MonoBehaviour
{
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

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveVector = Vector2.zero;

        moveAction = playerInput.actions["Move"];
        //tdControls.TD_BuilderControls.Move.performed += ctx => MoveCamera(ctx.ReadValue<Vector2>());
        moveAction.started += ctx => MoveCamera(ctx.ReadValue<Vector2>());
        moveAction.canceled += ctx => moveVector = Vector2.zero;


        //MoveCamera(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        targetZoom = cam.orthographicSize;
    }

    private void OnDisable()
    {
        //tdControls.TD_BuilderControls.Move.performed -= ctx => MoveCamera(ctx.ReadValue<Vector2>());
        //moveAction.canceled += ctx => moveVector = Vector2.zero;
        moveAction.started -= ctx => MoveCamera(ctx.ReadValue<Vector2>());
        moveAction.canceled -= ctx => moveVector = Vector2.zero;
    }

    void OnMove(InputValue value)
    {
        MoveCamera(value.Get<Vector2>());
    }

    private void MoveCamera(Vector2 moveValue)
    {
        if (moveValue == Vector2.zero && moveVector == Vector2.zero) return;
        moveVector = moveValue;
        //Debug.Log("MoveCamera" + moveValue + tdControls.TD_BuilderControls.Move.phase);
        Vector3 movementAmount = new Vector3(moveValue.x, moveValue.y, 0) * movementSpeed * Time.deltaTime;
        cam.transform.Translate(movementAmount);
    }

    // Update is called once per frame
    void Update()
    {
        if (moveAction.phase != InputActionPhase.Waiting)
        {
            MoveCamera(moveVector);
        }
        //else moveVector = Vector2.zero;

        targetZoom -= Input.mouseScrollDelta.y * sensitivity;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        float newSize = Mathf.MoveTowards(cam.orthographicSize, targetZoom, speed * Time.deltaTime);
        cam.orthographicSize = newSize;
    }
}
