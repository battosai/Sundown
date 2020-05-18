using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Game;

public class Villager : TownspersonClass, IHitboxResponder
{
    private readonly float RECOVERY_TIME = 5f;
	private Vector2[] ALARM = {new Vector2(0, 0), new Vector2(20, 16)};

    public void Start()
    {
        Reset();
        hitbox.SetResponder(this);
    }

    protected override void InitializeStateMachine()
    {
        Dictionary<Type, BaseState> states = new Dictionary<Type, BaseState>()
        {
            {typeof(IdleState), new IdleState(this)},
            {typeof(FleeState), new FleeState(this)}
        };
        stateMachine.SetStates(states);
    }

    public void Hit(Collider2D other, Act act)
    {
        switch(act)
        {
            case Act.ALARM:
                alarm(other);
                break;
            default:
                Debug.LogError($"Unknown Villager Act {act}");
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

    protected override void UpdateAnimator()
    {
        anim.SetBool("isAlarmed", isAlarmed);
        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x)+Mathf.Abs(rb.velocity.y));
    }

    public override void Reset()
    {
        maxHealth = 4;
        base.Reset();
    }
}