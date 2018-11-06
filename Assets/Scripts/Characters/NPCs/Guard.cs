using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : TownspersonClass, IHitboxResponder
{
    private readonly float RECOVERY_TIME = 5f;
    private readonly float AGGRO_LEASH = 25f;
    private readonly int FLEE_HEALTH = 1;
    private readonly int ATTACK_RANGE = 20;
    private readonly int TIME_BETWEEN_ATTACKS = 3;
	private Vector2[] ATTACK = {new Vector2(10, -5), new Vector2(10, 8)};
    private int strength = 2;

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
                    deathPrep(); 
                    break;
                case State.IDLE:
                    if(isAlarmed)
                    {
                        state = State.DEFEND;
                        goto case State.DEFEND;
                    } 
                    if(Time.time-time > 1f)
                    {
                        time = Time.time;
                        idleWalk(); //replace with patrol fnc later
                    }
                    break;
                case State.DEFEND:
                    if(health <= FLEE_HEALTH)
                    {
                        state = State.FLEE;
                        fleeToBarracks();
                        goto case State.FLEE;
                    }
                    //should have a periodic alarm signal when fighting
                    if(Vector2.Distance(floorPosition, player.floorPosition) > AGGRO_LEASH)
                    {
                        SetIsAlarmed(false);
                        state = State.IDLE;
                        goto case State.IDLE;    
                    }
                    rb.velocity = PathFinding.GetVelocity(floorPosition, player.floorPosition, speed);
                    if(Vector2.Distance(floorPosition, player.floorPosition) <= ATTACK_RANGE && Time.time-time > TIME_BETWEEN_ATTACKS)
                    {
                        //need some delay before the attack
                        Debug.Log(this.tag+" is attacking!");
                        attackCheck();
                        StartCoroutine(dash(player.floorPosition, WaitCallback));
                        time = Time.time;
                        //every attack do an alarm check?
                    }
                    break;
                case State.HOME:
                    if(Vector2.Distance(floorPosition, building.entrance.transform.position) <= ENTRANCE_RADIUS)
                    {
                        rb.velocity = Vector2.zero;
                        time = Time.time;
                        state = State.HIDE;
                        rend.enabled = false;
                        pushBox.enabled = false;
                        SetIsAlarmed(false);
                        goto case State.HIDE;
                    }
                    goto case State.FLEE;
                case State.FLEE:
                    //alarmCheck() ?
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
                    Debug.Log("[WARN]: Unknown Guard State "+state);
                    break;
            }
            UpdateAnimator();
            base.Update();
        }
    }
    
    private void fleeToBarracks()
    {
        StartCoroutine(takePath(building.entrance.transform.position, HomeCallback));
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
                Debug.Log("[WARN]: Unknown Guard Act "+act);
                break;
        }
    }

	private void attackCheck()
	{
		int dir = isLeft ? -1 : 1;
		hitbox.mask.useTriggers = false;
		hitbox.SetAct(Act.ATTACK);
		hitbox.SetOffset(new Vector2(ATTACK[0].x*dir, ATTACK[0].y));
		hitbox.SetSize(ATTACK[1]);
		hitbox.StartCheckingCollision();
		hitbox.CheckCollision();
		hitbox.StopCheckingCollision();
	}

    private void attack(Collider2D other)
    {
        if(other.tag == "Player")
		{
			PlayerClass player = other.GetComponent<PlayerClass>();
			player.SetAlarmPoint(player.floorPosition);
			Hurtbox hurtbox = other.GetComponent<Hurtbox>();	
			hurtbox.Hurt(strength, player);
		}
    }

    public override void UpdateAnimator()
    {
        anim.SetBool("isAlarmed", isAlarmed);
        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x)+Mathf.Abs(rb.velocity.y));
    }

    public override void Reset()
    {
        SetMaxHealth(6);
        base.Reset();
    }
}