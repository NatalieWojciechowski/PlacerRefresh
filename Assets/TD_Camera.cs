using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_Camera : MonoBehaviour
{

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
        if (animator)
        {
            animationVector = Vector2.SmoothDamp(animationVector, movementInput, ref animationVelocity, animationSmoothTime);
            animator.SetFloat("Vertical", animationVector.y);
            animator.SetFloat("Horizontal", animationVector.x);
        }
    }

    private void FixedUpdate()
    {
        float newH = Mathf.SmoothDamp(hCurrent, movementInput.x, ref hVelocity, smoothTime);
        float newV = Mathf.SmoothDamp(vCurrent, movementInput.y, ref vVecolity, smoothTime);
        hCurrent = newH;
        vCurrent = newV;

    }
}
