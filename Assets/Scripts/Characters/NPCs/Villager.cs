using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Villager : TownspersonClass, IHitboxResponder
{
    private readonly float RECOVERY_TIME = 5f;
	private Vector2[] ALARM = {new Vector2(0, 0), new Vector2(20, 16)};

    public override void Awake()
    {
        Reset();
    }

    public void Start()
    {
        hitbox.SetResponder(this);
    }

    public override void Update()
    {
        if(health <= 0)
            state = State.DEAD;
        if(isAlive)
        {
            switch(state)
            {
                case State.DEAD:
                    SetIsAlive(false);
                    rb.velocity = Vector2.zero;
                    break;
                case State.IDLE:
                    if(isAlarmed)
                    {
                        state = State.ALARM;
                        fleeToHome();
                        goto case State.ALARM;
                    }
                    if(Time.time-time > 1f)
                    {
                        time = Time.time;
                        idleWalk();
                    }
                    break;
                case State.ALARM:
                    if(Vector2.Distance(floorPosition, building.entrance.transform.position) <= ENTRANCE_RADIUS)
                    {
                        rb.velocity = Vector2.zero;
                        time = Time.time;
                        state = State.HIDE;
                        SetIsAlarmed(false);
                        rend.enabled = false;
                        pushBox.enabled = false;
                        goto case State.HIDE;
                    }
                    // if(Time.time-time > 1f)
                    // {
                    alarmCheck();
                    // }
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
                    break;
                default:
                    Debug.Log("[Error] Unrecognized State: "+state);
                    break;
            }
            UpdateAnimator();
            base.Update();
        }
    }

    public void Hit(Collider2D other, Action action)
    {
        switch(action)
        {
            case Action.ALARM:
                alarm(other);
                break;
            default:
                Debug.Log("[WARN]: Unknown Villager Action "+action);
                break;
        }
    }

   //triggers hitbox to check for travel colliders
	private void alarmCheck()
	{
		hitbox.mask.useTriggers = false;
		hitbox.SetAction(Action.ALARM);
		hitbox.SetOffset(ALARM[0]);
		hitbox.SetSize(ALARM[1]);
		hitbox.StartCheckingCollision();
		hitbox.CheckCollision();
		hitbox.StopCheckingCollision();
	}

    private void alarm(Collider2D other)
    {
        if(other.tag == "NPC")
        {
            CharacterClass npc = other.GetComponent<CharacterClass>();
            npc.SetAlarmPoint(floorPosition);
            npc.SetIsAlarmed(true);
        }
    }

    private void wanderInLeash()
    {
        //wander around assigned building entrance within leash
    }

    private void fleeToHome()
    {
        StartCoroutine(takePath(building.entrance.transform.position));
    }

    private void callGuards()
    {
        //alert nearby guards
    }

    public override void UpdateAnimator()
    {
        anim.SetBool("isAlarmed", isAlarmed);
        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x)+Mathf.Abs(rb.velocity.y));
    }

    public override void Reset()
    {
        SetMaxHealth(20);
        base.Reset();
    }
}