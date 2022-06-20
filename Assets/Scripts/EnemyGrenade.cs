using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGrenade : MonoBehaviour
{
    //���� ���� ��ü �ֺ� ���ΰ� ã�� ////////////////////
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
            Debug.Log("��Ҵ�.");
   
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
