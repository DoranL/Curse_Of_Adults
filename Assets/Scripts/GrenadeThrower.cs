using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeThrower : MonoBehaviour
{
    private float throwForce = 20f;
    public GameObject grenadePrefab;
    public GameObject player;

    //ThirdPersonMovement�� �����ϰ� CoolDown ��ũ��Ʈ���� cooldownTimer ���������� ����
    public GameObject skillUse2;

    private void Update()
    {
        //����ڰ� ���� ���콺�� ������ �� �� 3�� ���� Ű�� ������ �ʾ��� �� ThrowGrenade() ����
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
        rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);  // ForceMode.VelocityChange ���� �����ϰ� ��ü�� �ﰢ���� �ӵ� ������ �߰���
    }
}
