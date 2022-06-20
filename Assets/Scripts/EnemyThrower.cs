using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyThrower : MonoBehaviour
{
    private float throwForce = 15f;
    public GameObject EnemyThrowerPrefab;
    
    
    //적이 던지는 projectile이 전면
    private void EnemyThrowGrenade()
    {
        GameObject enemyGrenade = Instantiate(EnemyThrowerPrefab, transform.position, transform.rotation);
        Rigidbody rb = enemyGrenade.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
    }
}
