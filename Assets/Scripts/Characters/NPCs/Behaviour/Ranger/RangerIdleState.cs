using System;
using UnityEngine;

public class RangerIdleState : BaseState
{
    private static float CHANGE_ACTION_TIME = 1f;   //during non-arena
    private Ranger ranger;
    private float actionTimer;

    public RangerIdleState(Ranger ch) : base(ch)
    {
        this.ranger = ch;
    }

    public override Type Tick()
    {
        rb.velocity = Vector2.zero;
        actionTimer += Time.deltaTime;
        if(actionTimer > CHANGE_ACTION_TIME)
        {
            if(ranger.isArenaTime)
            {
                //decide which distance action to do
            }
            else
            {
                //check if should do chase/basic attack
                if(ranger.isAlarmed)
                    return typeof(RangerChaseState);
            }

            //idle motions
            int action = UnityEngine.Random.Range(0, 5);
            if(action == 0)
            {
                float horizontal = UnityEngine.Random.Range(-character.speed, character.speed);
                float vertical = UnityEngine.Random.Range(-character.speed, character.speed);
                rb.velocity = new Vector2(horizontal, vertical);
            }
        }
        return null;
    }
}