using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    public Animator animator;


    void Start()
    {
        animator = GetComponent<Animator>();


    }

    void Update()
    {

        animator.SetFloat("vertical", Input.GetAxis("Vertical"));
        animator.SetFloat("horizontal", Input.GetAxis("Horizontal"));
        //bool forwardPressed = Input.GetKey("w");
        //bool runPressed = Input.GetKey("left shift");


        ////player press w key
        //if (forwardPressed)
        //{
        //    animator.SetBool("isWalking", true);
        //}

        ////player not pressing w key
        //if (!forwardPressed)
        //{
        //    animator.SetBool("isWalking", false);
        //}

        //if ((forwardPressed && runPressed))
        //{
        //    animator.SetBool("isRunning", true);
        //}

        //if ((!forwardPressed || !runPressed))
        //{
        //    animator.SetBool("isRunning", false);
        //}

    }
}