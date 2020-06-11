using System;
using UnityEngine;

public class RangerTrishotState : BaseState
{
    private static readonly float ATTACK_TIME = 2f;
    private static readonly float ANGLE_DEVIATION = 5f*Mathf.Deg2Rad;
    private bool didAttack;
    private float attackTimer;
    private Ranger ranger;

    public RangerTrishotState(Ranger ch) : base(ch)
    {
        ranger = ch;
        didAttack = false;
        attackTimer = 0f;
    }

    public override Type Tick()
    {
        if(attackTimer < ATTACK_TIME)
            attackTimer += Time.deltaTime;
        else if(!didAttack)
        {
            didAttack = true;
            float angle = ranger.GetAngle(player.floorPosition);
            ranger.SpawnNeedle(angle);
            ranger.SpawnNeedle(angle+ANGLE_DEVIATION);
            ranger.SpawnNeedle(angle-ANGLE_DEVIATION);
        }
        else
        {
            attackTimer = 0f;
            didAttack = false;
            return typeof(RangerIdleState);
        }
        return null;
    }
}