using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Villager : TownspersonClass
{
    private readonly int leashX = 10;
    private readonly int leashY = 5;
    private readonly float alarmTimer = 5f;
    private bool isAlarmed;
    private Building building;
    private Vector2 leashPos;

    public void Start()
    {
        init();
    }

    public void Update()
    {
        if(isAlive)
        {
            switch(state)
            {
                case State.DEAD:
                    isAlive = false;
                    rb.velocity = Vector2.zero;
                    break;
                case State.IDLE:
                    break;
                case State.ALARM:
                    break;
                default:
                    Debug.Log("[Error] Unrecognized State: "+state);
                    break;
            }
            setFloorHeight();
            base.Update();
        }
    }

    public void Reset()
    {
        base.Reset();
        state = State.IDLE;
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