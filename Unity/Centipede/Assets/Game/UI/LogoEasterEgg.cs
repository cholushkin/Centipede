using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoEasterEgg : MonoBehaviour
{
    public float Chance;
    void Awake()
    {
        if(Random.value < Chance)
            gameObject.SetActive(false);
    }
}
