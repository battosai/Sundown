using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmallAnimal : Wildlife
{
    public override void Awake()
    {
        Reset();
    }
    public override void Reset()
    {
        SetMaxHealth(5);
        SetNutrition(1);
        SetClue(2);
        base.Reset();
    }
}