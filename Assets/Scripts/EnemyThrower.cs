using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyThrower : MonoBehaviour
{
    private float throwForce = 15f;
    public GameObject EnemyThrowerPrefab;
    
    
    //���� ������ projectile�� ����
    private void EnemyThrowGrenade()
    {
        GameObject enemyGrenade = Instantiate(EnemyThrowerPrefab, transform.position, transform.rotation);
        Rigidbody rb = enemyGrenade.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
    }
}
