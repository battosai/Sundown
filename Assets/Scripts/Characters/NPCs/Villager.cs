using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Villager : TownspersonClass
{
    private readonly int leashX = 10;
    private readonly int leashY = 5;
    private readonly float recoveryTime = 5f;
    private bool isAlarmed;
    private Building home;
    private Vector2 leashPos;
    public void SetHome(Building home){this.home=home;}

    public override void Awake()
    {
        // base.Init();
        Reset();
    }

    public override void Update()
    {
        if(isAlive)
        {
            switch(state)
            {
                case State.DEAD:
                    SetIsAlive(false);
                    rb.velocity = Vector2.zero;
                    break;
                case State.IDLE:
                    if(health < maxHealth)
                    {
                        state = State.ALARM;
                        List<Vector2> path = PathFinding.AStarJump(trans.position, home.entrance.transform.position, nodeMap, nodeID);
                        //do something with the path...coroutines? wouldn't need to make this an attribute then
                        goto case State.ALARM;
                    }
                    break;
                case State.ALARM:
                    break;
                case State.HIDE:
                    if(time-Time.time > recoveryTime)
                    {
                        state = State.IDLE;
                        goto case State.IDLE;
                    }
                    break;
                default:
                    Debug.Log("[Error] Unrecognized State: "+state);
                    break;
            }
            setFloorHeight();
            base.Update();
        }
    }

    public override void Reset()
    {
        state = State.IDLE;
        base.Reset();
    }

    private void wanderInLeash()
    {
        //wander around assigned building entrance within leash
    }

    private void fleeHome()
    {
        //enter building and board the door
    }

    private void callGuards()
    {
        //alert nearby guards
    }
}