using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject); // nie niszczy� podczas zmiany sceny
    }
}
