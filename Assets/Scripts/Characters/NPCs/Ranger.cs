using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : HeroClass, IHitboxResponder
{
    private readonly int MASTERY = 2;
    private readonly float AGGRO_RANGE = 100f;
    private readonly float ATTACK_RANGE = 200f;
	private Vector2 INTERACT_SIZE = new Vector2(50f, 50f);

	public override void Awake()
	{
		player = GameObject.Find("Player").GetComponent<PlayerClass>();
        hitBox = GetComponent<Hitbox>();
		base.Awake();
	}

    public void Start()
    {
        init();
        hitBox.SetResponder(this); 
        gameState.SetHero(this);
    }

    public override void Update()
    {
        if(isPresent)
        {
            switch(state)
            {
                case State.IDLE:
                    if(isAlarmed)
                    {
                        if(playerSpotted())
                        {
                            state = State.ATTACK;
                            goto case State.ATTACK;
                        }
                        Debug.Log("RANGER IS DOING AN INSPECT");
                        inspect();
                        //add inspect point to patrol path?
                        state = State.INSPECT;
                        goto case State.INSPECT;
                    } 
                    if(Time.time-time > 1f)
                    {
                        time = Time.time;
                        idleWalk();
                    }
                    break;
                case State.INSPECT:
                    interactCheck();
                    break;
                case State.ATTACK:
                    if(Vector2.Distance(floorPosition, player.floorPosition) < ATTACK_RANGE)
                        attackCheck();
                    break;
                default:
                    Debug.Log("[Error] Unknown Ranger Act: "+state);
                    break;
            }
        }
        base.Update();
    }

    private void inspect()
    {
        StartCoroutine(takePath(alarmPoint, null));
    }

    public override void Track(int nodeID)
    {
        Debug.Log("Ranger Mastery Track!"); 
        WorldNode wnode = World.wnodes[nodeID];
        SetLeads(leads+wnode.clues*tracking*MASTERY);
        presentInNode(false);
        if(leads >= PLAYER_FOUND)
        {
            Debug.Log("Ranger has found your current location!");
            presentInNode(true, nodeID+1);
            SetLeads(0);
        }
        Debug.Log("Ranger now has "+leads+" leads");
    }

    public override void Reset()
    {
        SetMaxHealth(50);
        SetLeads(0f);
        presentInNode(true, 0);
        tracking = 0.8f;
        visionRange = 100f;
        base.Reset();
    }

    public void Hit(Collider2D other, Act act)
    {
        switch(act)
        {
            case Act.INTERACT:
                interact(other);
                break;
            case Act.ATTACK:
                break;
            default:
                Debug.Log("[Error] Unrecognized Ranger Act");
                break;
        }
    }

    private void interactCheck()
    {
        hitBox.mask.useTriggers = false;
		hitBox.SetAct(Act.INTERACT);
		hitBox.SetOffset(floorPosition);
		hitBox.SetSize(INTERACT_SIZE);
		hitBox.StartCheckingCollision();
		hitBox.CheckCollision();
		hitBox.StopCheckingCollision();
    }

    private void attackCheck()
    {
        Debug.Log("RANGER DOES AN ATTACC");
    }

    //effectively the ranger's "scout" ability
    private void interact(Collider2D other)
    {
       
    }
}