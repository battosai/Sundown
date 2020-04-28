using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : HeroClass, IHitboxResponder
{
    public GameObject trap;
    public GameObject needle;
    private readonly int MASTERY = 2;
    private readonly int INSPECT_TIME = 10;
    private readonly int ATTACK_TIME = 3;
    private readonly int ATTACK_DMG = 2;
    private readonly int TRAP_MAX = 3;
    private readonly float TRISHOT_TIME = 3f;
    private readonly float REPOSITION_THRESHOLD = 10f;
    private readonly float REPOSITION_TIME = 5f;
    private readonly float LONG_DISTANCE = 100f;
    private readonly float MID_DISTANCE = 60f;
    private readonly float AGGRO_RANGE = 100f;
    private readonly float ATTACK_RANGE = 100f;
    private readonly float ATTACK_WIDTH = 2f;
    private readonly float TRISHOT_DEVIATION = 5f*Mathf.Deg2Rad;
    private readonly float TRITRAP_DEVIATION = 10f*Mathf.Deg2Rad;
    private enum Spacing {FAR, MID, CLOSE};
    private Spacing spacing;
	private Vector2 INTERACT_SIZE = new Vector2(50f, 50f);
    private List<GameObject> traps;
    private List<GameObject> needles;

	public override void Awake()
	{
		player = GameObject.Find("Player").GetComponent<PlayerClass>();
        hitBox = GetComponent<Hitbox>();
		base.Awake();
        gameState.SetHero(this);
	}

    public void Start()
    {
        init();
        traps = new List<GameObject>();
        needles = new List<GameObject>();
        hitBox.SetResponder(this); 
    }

    public override void Update()
    {
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
        float distance = Vector2.Distance(player.floorPosition, floorPosition);
        Spacing prevSpacing = spacing; 
        spacing = distance > LONG_DISTANCE ? Spacing.FAR : (distance > MID_DISTANCE ? Spacing.MID : Spacing.CLOSE);
        if(spacing != prevSpacing)
            time = Time.time;
        switch(spacing)
        {
            case Spacing.FAR:
                if(Time.time-time > TRISHOT_TIME)
                {
                    time = Time.time;
                    trishot();
                    Vector2 away = floorPosition-player.floorPosition;
                    StartCoroutine(dash(floorPosition+away));
                }
                break;
            case Spacing.MID:
                //maybe just place all 3 in a curve like trishot, and replace if a trap is missing
                //otherwise just triple shot?
                //maybe triple shot for midrange, pentashot for long range
                placeTrap();
                break;
            case Spacing.CLOSE:
                if(Time.time-time > REPOSITION_TIME)
                {
                    time = Time.time;
                    reposition();
                }
                break;
            default:
                Debug.Log("[Error] Unknown Spacing: "+spacing);
                break;
        }
    }

    private void reposition()
    {
        Vector2 repos = Arena.GetOpenPosition();
        trans.position = repos;
    }

    private void trishot()
    {
        Debug.Log("Trishot!");
        //midshot calculations
        float xdiff = player.floorPosition.x-floorPosition.x;
        float ydiff = player.floorPosition.y-floorPosition.y;
        float distance = Vector2.Distance(player.floorPosition, trans.position);
        float angle = ydiff > 0 ? Mathf.Acos(xdiff/distance) : -Mathf.Acos(xdiff/distance);
        float[] angles = new float[3]{angle, angle-TRISHOT_DEVIATION, angle+TRISHOT_DEVIATION};
        for(int i = 0; i < 3; i++)
            createNeedleOrUsePool(angles[i]);
    }

    private void createNeedleOrUsePool(float angle)
    {
        foreach(GameObject needle in needles)
            if(!needle.activeInHierarchy)
            {
                needle.SetActive(true);
                needle.transform.position = trans.position;
                needle.transform.rotation = Quaternion.Euler(0f, 0f, angle*Mathf.Rad2Deg);
                needle.GetComponent<Rigidbody2D>().velocity = needle.transform.right*Needle.SPEED;
                return;
            }
        GameObject newNeedle = Instantiate(needle, trans.position, Quaternion.Euler(0f, 0f, angle*Mathf.Rad2Deg));
        newNeedle.GetComponent<Rigidbody2D>().velocity = newNeedle.transform.right*Needle.SPEED;
        needles.Add(newNeedle);
    }

    private void tritrap()
    {
        float xdiff = (player.floorPosition.x-floorPosition.x)/2;
        float ydiff = (player.floorPosition.y-floorPosition.y)/2;
        float distance = Vector2.Distance(player.floorPosition, trans.position);
        float angle = ydiff > 0 ? Mathf.Acos(xdiff/distance) : -Mathf.Acos(xdiff/distance);
        float axdiff = distance*Mathf.Cos(angle+TRITRAP_DEVIATION);
        float aydiff = distance*Mathf.Sin(angle+TRITRAP_DEVIATION);
        float bxdiff = distance*Mathf.Cos(angle-TRITRAP_DEVIATION);
        float bydiff = distance*Mathf.Sin(angle-TRITRAP_DEVIATION);
        if(traps.Count == 0)
        {
            GameObject midtrap = Instantiate(trap, floorPosition + new Vector2(xdiff, ydiff), Quaternion.identity);
            GameObject atrap = Instantiate(trap, floorPosition + new Vector2(axdiff, aydiff), Quaternion.identity);
            GameObject btrap = Instantiate(trap, floorPosition + new Vector2(bxdiff, bydiff), Quaternion.identity);
            traps.Add(midtrap);
            traps.Add(atrap);
            traps.Add(btrap);
        }
        else
        {
            traps[0].SetActive(true);
            traps[1].SetActive(true);
            traps[2].SetActive(true);
            traps[0].transform.position = floorPosition + new Vector2(xdiff, ydiff);
            traps[1].transform.position = floorPosition + new Vector2(axdiff, aydiff);
            traps[2].transform.position = floorPosition + new Vector2(bxdiff, bydiff);
        }
    }

    private void placeTrap()
    {
        int active = 0;
        foreach(GameObject trap in traps)
            if(trap.activeInHierarchy)
                active++;
        if(active < TRAP_MAX)
        {
            GameObject newTrap;
            if(traps.Count < TRAP_MAX && traps.Count == active)
            {
                newTrap = Instantiate(trap);
                traps.Add(newTrap);
            }
            else
            {
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
        leads += wnode.clues*tracking*MASTERY;
        presentInNode(false);
        if(leads >= PLAYER_FOUND)
        {
            Debug.Log("Ranger has found your current location!");
            presentInNode(true, nodeID+1);
            leads = 0;
        }
        Debug.Log("Ranger now has "+leads+" leads");
    }

    public override void Reset()
    {
        maxHealth = 12;
        presentInNode(true, 0);
        leads = 0;
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