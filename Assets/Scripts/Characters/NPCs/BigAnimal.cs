using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BigAnimal : Wildlife
{
    public override void Awake()
    {
        base.Init();
        Reset();
    }
    public override void Reset()
    {
        SetMaxHealth(15);
        SetNutrition(5);
        SetBlood(0.5f);
        base.Reset();
    }
}