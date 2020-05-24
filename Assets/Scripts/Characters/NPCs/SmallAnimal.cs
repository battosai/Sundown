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
        maxHealth = 1;
        nutrition = 1;
        SetClue(2);
        base.Reset();
    }

    protected override void UpdateAnimator()
    {
        Debug.Log($"SmallAnimal.cs still needs to implement UpdateAnimator()");
    }
}