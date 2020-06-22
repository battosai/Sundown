using System;
using UnityEngine;

public class RangerRepositionState : BaseState
{
    private static readonly float REPOSITION_DELAY = 2f;
    private float timer;
    private Vector2 reposition;
    private Ranger ranger;

    public RangerRepositionState(Ranger ch) : base(ch)
    {
        ranger = ch;
        timer = 0f;
    }

    public override Type Tick()
    {
        if(timer > REPOSITION_DELAY)
        {
            timer = 0f;

            //maybe replace this method with a
            //more interesting fnc that finds
            //the relative "opposite" pos to place
            //the ranger
            reposition = Arena.GetOpenPosition();
            trans.position = reposition;
            return typeof(RangerIdleState);
        }
        else
            timer += Time.deltaTime;
        return null;
    }
}