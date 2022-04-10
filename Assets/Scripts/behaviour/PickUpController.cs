//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PickUpController : MonoBehaviour
//{
//    public ProjectileGunTutorial GunScript;
//    public Rigidbody rb;
//    public Transform player, GunContainer, fpsCam;

//    public float pickUpRange;
//    public float dropForwardForce, dropUpwardForce;

//    public bool equipped;
//    public static bool slotFull;

//    private void Update()
//    {
//        //check if player is in range and "E" is pressed
//        Vector3 distanceToPlayer = player.position - transform.position;
//        if (!equipped && distanceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E) && !slotFull) PickUp();

//        //drop if equipped and "Q" is pressed
//        if (equipped && Input.GetKeyDown(KeyCode.Q)) Drop();
//    }
    
//    private void PickUp()
//    {
//        equipped = true;
//        slotFull = true;

//        //make rigidbody kinematic and boxcollider a trigger
//        rb.isKinematic = true;
//        col.isTrigger = true;

//        //enable script
//        GunScript.enabled = true;
//    }

//    private void Drop()
//    {
//        equipped = false;
//        slotFull = false;

//        //make rigidbody not kinematic and boxcollider normal
//        rb.isKinematic = false;
//        col.isTrigger = false;

//        //disable script
//        GunScript.enabled = false;
//    }

//}
