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
        SetMaxHealth(3);
        SetNutrition(2);
        SetClue(4);
        base.Reset();
    }
}