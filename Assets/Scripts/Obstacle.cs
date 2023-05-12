using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject); // nie niszczyć podczas zmiany sceny
    }
}
