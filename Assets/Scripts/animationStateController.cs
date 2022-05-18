using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;

    int isWalkingHash;
    int isRunningHash;

    void Start()
    {
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
    }

    void Update()
    {
        bool isRunning = animator.GetBool(isRunningHash);
        bool isWalking = animator.GetBool(isWalkingHash);
        bool forwardPressed = Input.GetKey("w");
        bool runPressed = Input.GetKey("left shift");

        //player press w key
        if(!isWalking && forwardPressed)
        {
            animator.SetBool("isWalking", true);
        }

        //player not pressing w key
        if(isWalking && !forwardPressed)
        {
            animator.SetBool("isWalking", false);
        }

        if(!isRunning && (forwardPressed && runPressed))
        {
            animator.SetBool("isRunning", true);
        }

        if(isRunning &&(!forwardPressed || !runPressed))
        {
            animator.SetBool("isRunning", false);
        }
    }
}
