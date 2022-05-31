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
        countdown -= Time.deltaTime;   /////// 물체를 던지고 시간초 초과거나 폭발 상태가 아니면서 스킬키인 2를 눌럿을경우 폭발 근데 딜레이를 아무리 길게 줘도 똑같이 끝남

        if (countdown <= 0f ||( !hasExploded && (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha2))))
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
                       
                        GameObject target = colls[i].gameObject;
                        target.GetComponent<EnemyAiTutorial>().EnemySlow(9.0f);
                    }
                }
            }
        }
    }

    
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
