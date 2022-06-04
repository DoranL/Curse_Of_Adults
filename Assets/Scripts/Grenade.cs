using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float delay = 5.0f;
    public float radius = 5f;

    public float force = 200f;
    //effect
    public GameObject explosionEffect;
    
    float countdown;
    bool hasExploded = false;

    //던지는 오브젝트 주변 적 찾기
    public LayerMask Enemy;
    public float slowRange;
    public bool isSlowRange;

    void Start()
    {
        countdown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;

        if (countdown <= 0f ||( !hasExploded && (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))))
        {
            Explode();
            countdown = 0;
            hasExploded = true;
        }

        
        isSlowRange = Physics.CheckSphere(transform.position, slowRange, Enemy);
        if (isSlowRange)
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, slowRange);

            for(int i =0; i<colls.Length; i++)
            {
                if (colls[i].tag == "Enemy" && hasExploded)
                {
                    if (colls[i].name.Contains("Agile"))
                    {
                       //댜른 스크립트 접근 방법 1. 접근하고 싶은 스크립트를 가지고 있는 오브젝트를 가져오고(애자일) getcomponent를 통해 스크립트에 접근
                        GameObject target = colls[i].gameObject;
                        target.GetComponent<EnemyAiTutorial>().EnemySlow(9.0f);
                        target.GetComponent<EnemyAiTutorial>().animator.SetBool("isWalk", false);
                        target.GetComponent<EnemyAiTutorial>().hitGrenade = true;
                        
                    }
                }
            }
        }
    }

    //수류탄 가져오고 폭발하는 함수
    void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach(Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(force, transform.position, radius);
            }
        }
        
        Destroy(gameObject);
    }

    
}
