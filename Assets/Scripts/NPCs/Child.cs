using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Child : MonoBehaviour
{
    public Rigidbody2D rb {get; private set;}

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
}