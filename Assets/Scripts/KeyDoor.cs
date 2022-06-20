using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDoor : MonoBehaviour
{
    [SerializeField] private Key.KeyType keyType;
    public GameObject BringOpenKey;
    public Key.KeyType GetKeyType()
    {
        return keyType;
    }

    public void OpenDoor()
    {
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!BringOpenKey.GetComponent<KeyHolder>().ContainsKey(Key.KeyType.Gold))
            {
                if (GameObject.Find("Door").transform.Find("FindKeyText").gameObject)
                {
                    GameObject.Find("Door").transform.Find("FindKeyText").gameObject.SetActive(true);
                    Invoke("FalseText", 3f);
                }
            }
        }
    }

    public void FalseText()
    {
        GameObject.Find("Door").transform.Find("FindKeyText").gameObject.SetActive(false);
    }
}
