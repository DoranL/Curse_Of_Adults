using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeThrower : MonoBehaviour
{
    private float throwForce = 20f;
    public GameObject grenadePrefab;
    public GameObject player;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !player.GetComponent<ThirdPersonMovement>().isPressThree)
        {
            Invoke("ThrowGrenade", 0.4f);
        }
    }

    void ThrowGrenade()
    {
        GameObject grenade = Instantiate(grenadePrefab, transform.position, transform.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
        
    }
}
