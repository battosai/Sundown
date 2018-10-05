using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : HeroClass, IHitboxResponder
{
    public GameObject trap;
    private readonly int MASTERY = 2;
    private readonly int INSPECT_TIME = 10;
    private readonly int ATTACK_TIME = 3;
    private readonly int ATTACK_DMG = 5;
    private readonly int TRAP_MAX = 3;
    private readonly float AGGRO_RANGE = 100f;
    private readonly float ATTACK_RANGE = 100f;
    private readonly float ATTACK_WIDTH = 2f;
    private readonly float TRISHOT_DEVIATION = 5f*Mathf.Deg2Rad;
    private enum ArenaState {TRISHOT, TRAP, REPOSITION};
    private ArenaState arenaState;
	private Vector2 INTERACT_SIZE = new Vector2(50f, 50f);
    private List<GameObject> traps;

	public override void Awake()
	{
		player = GameObject.Find("Player").GetComponent<PlayerClass>();
        hitBox = GetComponent<Hitbox>();
		base.Awake();
	}

    public void Start()
    {
        init();
        traps = new List<GameObject>();
        hitBox.SetResponder(this); 
        gameState.SetHero(this);
    }

    public override void Update()
    {
        if(PlayerInput.Space)
            placeTrap();
        if(!isArenaTime)
        {
            if(isPresent)
            {
                switch(state)
                {
                    case State.IDLE:
                        if(isAlarmed)
                        {
                            //attack if found player, or attacked directly
                            //health will only go down if attacked by player
                            if(playerSpotted() || health < maxHealth)
                            {
                                state = State.ATTACK;
                                goto case State.ATTACK;
                            }
                            Debug.Log("RANGER IS DOING AN INSPECT");
                            StartCoroutine(takePath(alarmPoint, InspectCallback));
                            state = State.COROUTINE;
                            goto case State.COROUTINE;
                        } 
                        if(Time.time-time > 1f)
                        {
                            time = Time.time;
                            idleWalk();
                        }
                        break;
                    case State.INSPECT:
                        if(Time.time-time > INSPECT_TIME)
                        {
                            state = State.IDLE;
                            goto case State.IDLE;
                        } 
                        interactCheck();
                        break;
                    case State.COROUTINE:
                        break;
                    case State.ATTACK:
                        float distance = Vector2.Distance(floorPosition, player.floorPosition);
                        if(distance > AGGRO_RANGE)
                        {
                            Debug.Log("Player has escaped the Ranger");
                            SetIsAlarmed(false);
                            state = State.IDLE;
                            goto case State.IDLE;
                        }
                        if(distance < ATTACK_RANGE && Time.time-time > ATTACK_TIME)
                        {
                            time = Time.time;
                            basicAttack();
                        }
                        break;
                    default:
                        Debug.Log("[Error] Unknown Ranger Act: "+state);
                        break;
                }
            }
        }
        else
        {
            ArenaUpdate();
        }
        base.Update();
    }

    public override void ArenaUpdate()
    {
        //based on distance can change roll values for abilities?
        switch(arenaState)
        {
            case ArenaState.TRISHOT:
                trishot();
                break;
            case ArenaState.TRAP:
                placeTrap();
                break;
            case ArenaState.REPOSITION:
                break;
            default:
                Debug.Log("[Error] Unknown Ranger Arena State: "+arenaState);
                break;
        }
    }

    private void trishot()
    {
        //midshot calculations
        float xdiff = player.floorPosition.x-floorPosition.x;
        float ydiff = player.floorPosition.y-floorPosition.y;
        float distance = Vector2.Distance(player.floorPosition, floorPosition);//Mathf.Sqrt(xdiff*xdiff+ydiff*ydiff);
        float angle = ydiff > 0 ? Mathf.Acos(xdiff/distance) : -Mathf.Acos(xdiff/distance);
        //sideshot calculations 
        float xOffset = distance*Mathf.Cos(angle-TRISHOT_DEVIATION);
        float yOffset = distance*Mathf.Sin(angle-TRISHOT_DEVIATION);
        Vector2 sideshotA = new Vector2(floorPosition.x+xOffset, floorPosition.y+yOffset);
        xOffset = distance*Mathf.Cos(angle+TRISHOT_DEVIATION);
        yOffset = distance*Mathf.Sin(angle+TRISHOT_DEVIATION);
        Vector2 sideshotB = new Vector2(floorPosition.x+xOffset, floorPosition.y+yOffset);
        //perform 3 raycasts
        RaycastHit2D midhit = Physics2D.CircleCast(floorPosition, ATTACK_WIDTH, player.floorPosition-floorPosition, ATTACK_RANGE);
        Debug.DrawRay(floorPosition, player.floorPosition-floorPosition, Color.cyan, 2f);
        RaycastHit2D sidehitA = Physics2D.CircleCast(floorPosition, ATTACK_WIDTH, sideshotA-floorPosition, ATTACK_RANGE);
        Debug.DrawRay(floorPosition, sideshotA-floorPosition, Color.red, 2f);
        RaycastHit2D sidehitB = Physics2D.CircleCast(floorPosition, ATTACK_WIDTH, sideshotB-floorPosition, ATTACK_RANGE);
        Debug.DrawRay(floorPosition, sideshotB-floorPosition, Color.red, 2f);
        int hits = 0;
        if(midhit.collider != null && midhit.collider.tag == "Player")
            hits++;
        if(sidehitA.collider != null && sidehitA.collider.tag == "Player")
            hits++;
        if(sidehitB.collider != null && sidehitB.collider.tag == "Player")
            hits++;
        if(hits > 0)
            player.hurtBox.Hurt(ATTACK_DMG*hits); 
    }

    private void placeTrap()
    {
        int active = 0;
        foreach(GameObject trap in traps)
            if(trap.activeInHierarchy)
                active++;
        Debug.Log("Active: "+active);
        if(active < TRAP_MAX)
        {
            GameObject newTrap;
            if(traps.Count < TRAP_MAX && traps.Count == active)
            {
                Debug.Log("Creating new trap");
                newTrap = Instantiate(trap);
                traps.Add(newTrap);
            }
            else
            {
                Debug.Log("Using trap pool");
                newTrap = traps[active]; 
                newTrap.SetActive(true);
            }
            Vector2 offset = (player.floorPosition-floorPosition)/2;
            Vector2 pos = floorPosition+offset;
            newTrap.transform.position = pos;
        }
    }

    public void InspectCallback()
    {
        state = State.INSPECT;
        time = Time.time;
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

    private void basicAttack()
    {
        Debug.Log("Ranger is attacking!");
        RaycastHit2D hit = Physics2D.CircleCast(floorPosition, ATTACK_WIDTH, player.floorPosition-floorPosition, ATTACK_RANGE);
        if(hit.collider != null)
        {
            if(hit.collider.tag == "Player")
                player.hurtBox.Hurt(ATTACK_DMG);
        }
    }

    //effectively the ranger's "scout" ability
    private void interact(Collider2D other)
    {
       
    }
}