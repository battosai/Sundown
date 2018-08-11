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
                        goto case State.ALARM;
                    }
                    break;
                case State.ALARM:
                    //travel on path and alert ppl on the way                
                    if(trans.position == home.entrance.transform.position)
                    {
                        isAlarmed = false;
                        state = State.HIDE;
                        goto case State.HIDE;
                    }
                    if(!isAlarmed)
                    {
                        Debug.Log("Running home!");
                        StartCoroutine(takePath(home.entrance.transform.position));
                        isAlarmed = true;
                    }
                    break;
                case State.HIDE:
                    if(time-Time.time > recoveryTime)
                    {
                        SetHealth(maxHealth);
                        rend.enabled = true;
                        pushBox.enabled = true;
                        state = State.IDLE;
                        goto case State.IDLE;
                    }
                    rend.enabled = false;
                    pushBox.enabled = false;
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
        isAlarmed = false;
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