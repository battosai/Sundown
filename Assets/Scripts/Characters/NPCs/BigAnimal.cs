using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BigAnimal : Wildlife
{
    public override void Awake()
    {
        Reset();
    }
    public override void Reset()
    {
        SetMaxHealth(15);
        SetNutrition(5);
        SetClue(4);
        SetBlood(0.5f);
        base.Reset();
    }
}