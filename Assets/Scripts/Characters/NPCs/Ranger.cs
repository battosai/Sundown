using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : HeroClass
{
    public readonly int FOLLOW = 15;
    public readonly int MASTERY = 2;

    public void Start()
    {
        gameState.SetHero(this);
    }

    public void Update()
    {
        setFloorHeight();
    }
    public override void Track()
    {
       Debug.Log("Ranger Mastery Track!"); 
    }
}