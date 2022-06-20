using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFollow : MonoBehaviour
{
    public GameObject thePlayer;

    void Update()
    {
        transform.LookAt(thePlayer.transform);
    }
}
