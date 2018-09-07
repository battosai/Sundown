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
                case State.HOME:
                    //change how this interacts with the takePath subroutine, this is messy
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
                    goto case State.ALARM;
                case State.ALARM:
                    alarmCheck();
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
                    rb.velocity = Vector2.zero;
                    break;
                default:
                    Debug.Log("[Error] Unrecognized State: "+state);
                    break;
            }
            UpdateAnimator();
            base.Update();
        }
    }

    public void Hit(Collider2D other, Act act)
    {
        switch(act)
        {
            case Act.ALARM:
                alarm(other);
                break;
            default:
                Debug.Log("[WARN]: Unknown Villager Act "+act);
                break;
        }
    }

   //triggers hitbox to check for travel colliders
	private void alarmCheck()
	{
		hitbox.mask.useTriggers = false;
		hitbox.SetAct(Act.ALARM);
		hitbox.SetOffset(ALARM[0]);
		hitbox.SetSize(ALARM[1]);
		hitbox.StartCheckingCollision();
		hitbox.CheckCollision();
		hitbox.StopCheckingCollision();
	}

    private void alarm(Collider2D other)
    {
        if(other.tag == "Wildlife")
        {
            CharacterClass animal = other.GetComponent<CharacterClass>();
            animal.SetAlarmPoint(floorPosition);
            animal.SetIsAlarmed(true);
        }
        else if(other.tag == "NPC" || other.tag == "Hero")
        {
            CharacterClass character = other.GetComponent<CharacterClass>();
            character.SetAlarmPoint(alarmPoint);
            character.SetIsAlarmed(true);
        }
    }

    private void wanderInLeash()
    {
        //wander around assigned building entrance within leash
    }

    private void fleeToHome()
    {
        StartCoroutine(takePath(building.entrance.transform.position, HomeCallback));
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