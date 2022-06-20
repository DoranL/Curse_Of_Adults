using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeThrower : MonoBehaviour
{
    private float throwForce = 20f;
    public GameObject grenadePrefab;
    public GameObject player;

    //ThirdPersonMovement랑 동일하게 CoolDown 스크립트에서 cooldownTimer 가져오려고 선언
    public GameObject skillUse2;

    private void Update()
    {
        //사용자가 왼쪽 마우스를 누르고 그 때 3번 숫자 키를 누르지 않았을 때 ThrowGrenade() 실행
        if (Input.GetMouseButtonDown(0) && !player.GetComponent<ThirdPersonMovement>().isPressThree && (skillUse2.GetComponent<CoolDown>().cooldownTimer == 0.0f))
        {
            player.GetComponent<ThirdPersonMovement>().animator.SetBool("isThrow", true);
            skillUse2.GetComponent<CoolDown>().UseSkill();
            Invoke("ThrowGrenade", 0.4f);
        }
    }

    void ThrowGrenade()
    {
        GameObject grenade = Instantiate(grenadePrefab, transform.position, transform.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);  // ForceMode.VelocityChange 질량 무시하고 강체에 즉각적인 속도 변경을 추가함
    }
}
