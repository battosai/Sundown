using System;
using System.Collections.Generic;
using UnityEngine;

public class RangerAttackState : BaseState
{
    private static readonly float ATTACK_TIME = 2f;
    private bool didAttack;
    private float attackTimer;
    private Ranger ranger;

    public RangerAttackState(Ranger ch) : base(ch)
    {
        ranger = ch;
        didAttack = false;
        attackTimer = 0f;
    }

    public override Type Tick()
    {
        //will wait 2 seconds (lining up shot) before shooting
        if(attackTimer < ATTACK_TIME)
            attackTimer += Time.deltaTime;
        else if(!didAttack)
        {
            didAttack = true;
            ranger.SpawnNeedle(ranger.GetAngle(player.floorPosition));
        }
        else
        {
            attackTimer = 0f;
            didAttack = false;
            return typeof(RangerChaseState);
        }
        return null;
    }
}