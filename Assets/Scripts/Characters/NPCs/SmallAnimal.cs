using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmallAnimal : Wildlife
{
    public void Awake()
    {
        Reset();
    }
    public  void Reset()
    {
        Debug.Log("SmallAnimal Reset");
        base.Reset();
        SetHealth(5);
        SetNutrition(1);
        SetBlood(0.2f);
    }
}