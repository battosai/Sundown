using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Container : MonoBehaviour
{
    public bool isEmpty;
    public int gold {get; private set;} 

    public void Start()
    {
        gold = Random.Range(0, 10);
        isEmpty = false;
    }
}