using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public int health;
    public int numOfHearts;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private void Start()
    {
        numOfHearts = 3;
    }

    void Update()
    {
        
    }


    public void DecreaseHeart()
    {
        numOfHearts -= 1;
        
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < numOfHearts)
            {
                hearts[i].sprite = fullHeart;
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
            /////////////////////////////////////¾À ÀüÈ¯ ³ªÁß¿¡ ²À Å°¼À
            //if(numOfHearts == 0)
            //{
            //    SceneManager.LoadScene(3);
            //}
        }
    }
}
