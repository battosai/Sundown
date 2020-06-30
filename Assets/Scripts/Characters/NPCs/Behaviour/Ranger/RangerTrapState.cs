using System;
using UnityEngine;

public class RangerTrapState : BaseState
{
    private static readonly float ATTACK_TIME = 1f;
    private static readonly float ANGLE_DEVIATION = 10f*Mathf.Deg2Rad;
    private bool didAttack;
    private float attackTimer;
    private Ranger ranger;
    public RangerTrapState(Ranger ch) : base(ch)
    {
        ranger = ch;
        didAttack = false;
        attackTimer = 0f;
    }

    public override Type Tick()
    {
        if(ranger.traps.Count < ranger.TRAP_MAX)
        {
            if(attackTimer < ATTACK_TIME)
                attackTimer += Time.deltaTime;
            else if(!didAttack)
            {
                didAttack = true;
                Vector2 midpt = 
                    new Vector2(player.floorPosition.x+trans.position.x, 
                    player.floorPosition.y+trans.position.y)/2;
                float angle = ranger.GetAngle(midpt);
                float distance = Vector2.Distance(midpt, trans.position);
                float axDiff = distance*Mathf.Cos(angle+ANGLE_DEVIATION);
                float ayDiff = distance*Mathf.Sin(angle+ANGLE_DEVIATION);
                float bxDiff = distance*Mathf.Cos(angle-ANGLE_DEVIATION);
                float byDiff = distance*Mathf.Sin(angle-ANGLE_DEVIATION);
                ranger.SpawnTrap(midpt);
                ranger.SpawnTrap(ranger.floorPosition + new Vector2(axDiff, ayDiff));
                ranger.SpawnTrap(ranger.floorPosition + new Vector2(bxDiff, byDiff));
            }
            else
            {
                attackTimer = 0f;
                didAttack = false;
                return typeof(RangerIdleState);
            }
            return null;
        }
        //shoot player if too many traps
        return typeof(RangerTrishotState);
    }
}