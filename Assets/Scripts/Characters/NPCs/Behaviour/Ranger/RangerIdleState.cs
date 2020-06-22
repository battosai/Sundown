using System;
using UnityEngine;

public class RangerIdleState : BaseState
{
    //arena constants
    private static readonly float LONG_RANGE = Mathf.Pow(100, 2);
    private static readonly float MID_RANGE = Mathf.Pow(50, 2);
    private static readonly float REPOSITION_TOLERANCE = Mathf.Pow(10, 2);

    //non arena constants
    private static readonly float CHANGE_ACTION_TIME = 1f;   //during non-arena

    private Ranger ranger;
    private float actionTimer;

    public RangerIdleState(Ranger ch) : base(ch)
    {
        this.ranger = ch;
    }

    public override Type Tick()
    {
        if(ranger.isArenaTime)
        {
            //decide which distance action to do
            float dist = (player.floorPosition-ranger.floorPosition).sqrMagnitude; 
            if(dist > LONG_RANGE)
                return typeof(RangerTrishotState);
            else if(dist > MID_RANGE)
                return typeof(RangerTrapState);
            else if(dist > REPOSITION_TOLERANCE)
                return typeof(RangerRepositionState);
            else
                return typeof(RangerTrishotState);
        }
        else
        {
            //check if should do chase/basic attack
            if(ranger.isAlarmed)
                return typeof(RangerChaseState);
            actionTimer += Time.deltaTime;
            if(actionTimer > CHANGE_ACTION_TIME)
            {
                //idle motions
                int action = UnityEngine.Random.Range(0, 5);
                if(action == 0)
                {
                    float horizontal = UnityEngine.Random.Range(-character.speed, character.speed);
                    float vertical = UnityEngine.Random.Range(-character.speed, character.speed);
                    rb.velocity = new Vector2(horizontal, vertical);
                }
                else
                    rb.velocity = Vector2.zero;
                actionTimer = 0f;
            }
        }
        return null;
    }
}