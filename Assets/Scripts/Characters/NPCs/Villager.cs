using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Villager : TownspersonClass
{
    private Building building;
    private Vector2 leashPos;
    private readonly int leashY = 5;
    private readonly int leashX = 10;

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