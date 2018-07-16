using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmallAnimal : Wildlife
{
    public override void Awake()
    {
        base.Init();
        Reset();
    }
    public override void Reset()
    {
        SetMaxHealth(5);
        SetNutrition(1);
        SetClue(2);
        SetBlood(0.2f);
        base.Reset();
    }
}