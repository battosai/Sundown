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
        maxHealth = 3;
        nutrition = 2;
        SetClue(4);
        base.Reset();
    }

    protected override void UpdateAnimator()
    {
        Debug.Log($"BigAnimal.cs still needs to implement UpdateAnimator()");
    }
}