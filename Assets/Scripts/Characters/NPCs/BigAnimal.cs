using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BigAnimal : Wildlife
{
    public void Awake()
    {
        Reset();
    }
    public override void Reset()
    {
        base.Reset();
        SetHealth(15);
        SetNutrition(5);
        SetBlood(0.5f);
    }
}