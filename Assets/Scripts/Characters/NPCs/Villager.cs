using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Villager : TownspersonClass
{
    private readonly float RECOVERY_TIME = 5f;
    private bool isAlarmed;
    private Building home;
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
                    if(Time.time-time > 1f)
                    {
                        time = Time.time;
                        idleWalk();
                    }
                    break;
                case State.ALARM:
                    if(Vector2.Distance(floorPosition, home.entrance.transform.position) <= 1f)
                    {
                        isAlarmed = false;
                        rb.velocity = Vector2.zero;
                        time = Time.time;
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
                    if(Time.time-time > RECOVERY_TIME)
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
            UpdateAnimator();
            base.Update();
        }
    }

    public override void UpdateAnimator()
    {
        anim.SetBool("isAlarmed", isAlarmed);
        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x)+Mathf.Abs(rb.velocity.y));
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