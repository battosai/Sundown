using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class Guard : TownspersonClass, IHitboxResponder
{
    public static readonly float AGGRO_LEASH = 75f; //make sure this is larger than the radius of werewolfaggrobox
    public static readonly int ATTACK_RANGE = 20;
	public static readonly Vector2[] ATTACK = {new Vector2(10, -5), new Vector2(10, 8)};
    private static readonly int strength = 1;

    public override void Awake()
    {
        Reset();
    }    

    public void Start()
    {
        hitbox.SetResponder(this);
    }

    protected override void InitializeStateMachine()
    {
        Dictionary<Type, BaseState> states = new Dictionary<Type, BaseState>()
        {
            {typeof(IdleState), new IdleState(this)},
            {typeof(FleeState), new FleeState(this)},
            {typeof(ChaseState), new ChaseState(this)},
            {typeof(AttackState), new AttackState(this)},
            {typeof(RestState), new RestState(this)}
        };
        stateMachine.SetStates(states);
    }

    public void Hit(Collider2D other, Act act)
    {
        switch(act)
        {
            case Act.ATTACK:
                attack(other);
                break;
            case Act.INTERACT:
                break;
            case Act.ALARM:
                break;
            default:
                Debug.LogError($"Unknown Guard Act {act}");
                break;
        }
    }

    private void attack(Collider2D other)
    {
        if(other.tag == "Player")
		{
			PlayerClass player = other.GetComponent<PlayerClass>();
			// player.SetAlarmPoint(player.floorPosition);
			Hurtbox hurtbox = other.GetComponent<Hurtbox>();	
			hurtbox.Hurt(strength, player);
		}
    }

    protected override void UpdateAnimator()
    {
        anim.SetBool("isAlarmed", isAlarmed);
        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x)+Mathf.Abs(rb.velocity.y));
    }

    public override void Reset()
    {
        maxHealth = 6;
        base.Reset();
    }
}