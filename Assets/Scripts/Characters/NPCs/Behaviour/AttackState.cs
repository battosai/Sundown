using System;
using UnityEngine;
using Game;

//attacks for guards
public class AttackState : BaseState
{
    private static float DASH_TIME = 0.2f;
    private static float DASH_SPEED = 40f;
    private static float RECOVERY_TIME = 1f;
    private PlayerClass player;
    private Hitbox hitbox;
    private bool didAttack;
    private float dashTimer;
    private float recoveryTimer;

    public AttackState(TownspersonClass ch) : base(ch)
    {
        this.player = GameObject.Find("Player").GetComponent<PlayerClass>();
        this.hitbox = ch.hitbox;
        didAttack = false;
        dashTimer = 0f;
        recoveryTimer = 0f;
    }

    public override Type Tick()
    {
		float dist = Vector2.Distance(character.floorPosition, player.floorPosition);

        //if too far, chase
        if(dist > Guard.ATTACK_RANGE)
        {
            Reset();
            return typeof(ChaseState);
        }
        
        //attack, dash, wait, return to chase state
        if(!didAttack)
            AttackCheck();
        else if(dashTimer < DASH_TIME)
        {
            dashTimer += Time.deltaTime;
			rb.velocity = PathFinding.GetVelocity(character.floorPosition, player.floorPosition, DASH_SPEED);
        }
        else if(recoveryTimer < RECOVERY_TIME)
        {
            rb.velocity = Vector2.zero;
            recoveryTimer += Time.deltaTime;
        }
        else
        {
            Reset();
            return typeof(ChaseState);
        }
        return null;
    }

    private void Reset()
    {
        didAttack = false;
        dashTimer = 0f;
        recoveryTimer = 0f;
    }

	private void AttackCheck()
	{
		int dir = character.isLeft ? -1 : 1;
		hitbox.mask.useTriggers = false;
		hitbox.SetAct(Act.ATTACK);
		hitbox.SetOffset(new Vector2(Guard.ATTACK[0].x*dir, Guard.ATTACK[0].y));
		hitbox.SetSize(Guard.ATTACK[1]);
		hitbox.StartCheckingCollision();
		hitbox.CheckCollision();
		hitbox.StopCheckingCollision();
        didAttack = true;
	}
}