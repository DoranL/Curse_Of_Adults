using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGrenade : MonoBehaviour
{
    //적이 던진 물체 주변 주인공 찾기 ////////////////////
    public LayerMask Player;
    public float damageRange;
    public bool isDamageRange;
    public GameObject playerTarget;

    // Update is called once per frame
    private void Start()
    {
        Invoke("DecreaseHeartPlayer", 0.5f);
    }

    void DecreaseHeartPlayer()
    {
        isDamageRange = Physics.CheckSphere(transform.position, damageRange, Player); 
        if (isDamageRange)
        {
            Debug.Log("잡았다.");
   
            playerTarget = GameObject.Find("Third Person Player");
            playerTarget.GetComponent<Health>().DecreaseHeart();
            
        }
        Invoke("DestroyProjectile", 0.5f);
    }

    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
