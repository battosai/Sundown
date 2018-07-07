using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmallAnimal : Wildlife
{
    public void Awake()
    {
        Reset();
    }
    public override void Reset()
    {
        SetMaxHealth(5);
        SetNutrition(1);
        SetBlood(0.2f);
        base.Reset();
    }
}