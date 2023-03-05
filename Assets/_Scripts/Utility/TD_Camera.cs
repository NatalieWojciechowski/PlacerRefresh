using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TD_Camera : MonoBehaviour
{
    public float minX = -40;
    public float minZ = -40;
    public float maxX = 40;
    public float maxZ = 40;

    private float animationSmoothTime = 0.3f;
    private Vector2 animationVector;
    private Vector2 animationVelocity;

    Vector2 movementInput;
    float smoothTime = 0.3f;
    float hVelocity = 0f;
    float vVecolity = 0f;
    float hCurrent = 0f;
    float vCurrent = 0f;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        verifyBounds();
        if (animator)
        {
            animationVector = Vector2.SmoothDamp(animationVector, movementInput, ref animationVelocity, animationSmoothTime);
            animator.SetFloat("Vertical", animationVector.y);
            animator.SetFloat("Horizontal", animationVector.x);
        }
    }

    private void verifyBounds()
    {
        Vector3 boundPosition = transform.position;
        if (boundPosition.x < minX) boundPosition.x = minX;
        else if (boundPosition.x > maxX) boundPosition.x = maxX;

        if (boundPosition.z < minZ) boundPosition.z = minZ;
        else if (boundPosition.z > maxZ) boundPosition.z = maxZ;

        transform.position = boundPosition;
        //transform.position.Set(boundPosition.x, boundPosition.y, boundPosition.z);
    }

    private void FixedUpdate()
    {
        float newH = Mathf.SmoothDamp(hCurrent, movementInput.x, ref hVelocity, smoothTime);
        float newV = Mathf.SmoothDamp(vCurrent, movementInput.y, ref vVecolity, smoothTime);
        hCurrent = newH;
        vCurrent = newV;

    }
}
