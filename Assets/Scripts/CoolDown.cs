using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CoolDown : MonoBehaviour
{
    [SerializeField] 
    private Image imageCooldown;
    [SerializeField]
    private TMP_Text textCooldown;

    //��ٿ� Ÿ�̸� ����
    private bool isCooldown = false;
    public float cooldownTime = 10.0f;
    public float cooldownTimer = 0.0f;

    //ThirdPersonMovement�� 
    private void Start()
    {
        textCooldown.gameObject.SetActive(false);
        imageCooldown.fillAmount = 0.0f;
    }

    private void Update()
    {
        if (isCooldown)
        {
            ApplyCooldown();
        }
        
    }
    
    public void ApplyCooldown()
    {
        //subtrack time since last called
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0.0f)
        {
            isCooldown = false;
            textCooldown.gameObject.SetActive(false);
            imageCooldown.fillAmount = 0.0f;
            cooldownTimer = 0.0f;
        }
        else
        {
            textCooldown.text = Mathf.RoundToInt(cooldownTimer).ToString();
            imageCooldown.fillAmount = cooldownTimer / cooldownTime;
        }
    }

    public void UseSkill()
    {
        if (isCooldown)
        {
            //return false;
        }
        else
        {
            isCooldown = true;
            textCooldown.gameObject.SetActive(true);
            cooldownTimer = cooldownTime;
        }
    }
}
