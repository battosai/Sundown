using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class Ranger : HeroClass, IHitboxResponder
{
	public static readonly Vector2[] SHOVE = {new Vector2(10, -5), new Vector2(10, 8)};
    public static readonly float SHOVE_FORCE = 10f;
    public static readonly float SHOVE_DURATION = 1f;
    public Trap trap;
    public Needle needle;
    private readonly int MASTERY = 2;
    public readonly int TRAP_MAX = 6;
    public List<Trap> traps {get; private set;}
    private List<Needle> needles;

	public override void Awake()
	{
		player = GameObject.Find("Player").GetComponent<PlayerClass>();
        hitBox = GetComponent<Hitbox>();
        stateMachine = GetComponent<StateMachine>();
		base.Awake();
        gameState.SetHero(this);
	}

    public void Start()
    {
        traps = new List<Trap>();
        needles = new List<Needle>();
        hitBox.SetResponder(this); 
        InitializeStateMachine();
    }

    protected override void InitializeStateMachine()
    {
        Dictionary<Type, BaseState> states = new Dictionary<Type, BaseState>()
        {
            {typeof(RangerIdleState), new RangerIdleState(this)},
            {typeof(RangerChaseState), new RangerChaseState(this)},
            {typeof(RangerAttackState), new RangerAttackState(this)},
            {typeof(RangerTrishotState), new RangerTrishotState(this)},
            {typeof(RangerTrapState), new RangerTrapState(this)},
            {typeof(RangerRepositionState), new RangerRepositionState(this)}
        };
        stateMachine.SetStates(states);
    }

    //places a needle (from pool or newly created) to fire at a certain angle
    public void SpawnNeedle(float angle)
    {
        foreach(Needle n in needles)
            if(!n.gameObject.activeInHierarchy)
            {
                n.gameObject.SetActive(true);
                n.transform.position = trans.position;
                n.transform.rotation = Quaternion.Euler(0f, 0f, angle*Mathf.Rad2Deg);
                n.rb.velocity = n.transform.right*Needle.SPEED;
                return;
            }
        Needle newNeedle = Instantiate(needle, trans.position, Quaternion.Euler(0f, 0f, angle*Mathf.Rad2Deg));
        newNeedle.rb.velocity = newNeedle.transform.right*Needle.SPEED;
        needles.Add(newNeedle);
    }

    //places a trap (from pool or newly created) at position
    public void SpawnTrap(Vector3 position)
    {
        foreach(Trap t in traps)
        {
            if(!t.gameObject.activeInHierarchy)
            {
                t.gameObject.SetActive(true);
                t.transform.position = position;
                return;
            }
        }
        if(traps.Count < TRAP_MAX)
        {
            Trap newTrap = Instantiate(trap, position, Quaternion.identity);
            traps.Add(newTrap);
        }
    }

    public float GetAngle(Vector3 target)
    {
        float xdiff = target.x - trans.position.x;
        float ydiff = target.y - trans.position.y;
        float distance = Vector2.Distance(target, trans.position);
        float angle = ((ydiff > 0) ? 1 : -1) * Mathf.Acos(xdiff/distance);
        return angle;
    }

    public override void Track(int nodeID)
    {
        Debug.Log("Ranger Mastery Track!"); 
        WorldNode wnode = World.wnodes[nodeID];
        leads += wnode.clues*tracking*MASTERY;
        PresentInNode(false);
        if(leads >= PLAYER_FOUND)
        {
            Debug.Log("Ranger has found your current location!");
            PresentInNode(true, nodeID+1);
            leads = 0;
        }
        Debug.Log("Ranger now has "+leads+" leads");
    }

    public override void Reset()
    {
        maxHealth = 12;
        PresentInNode(true, 0);
        leads = 0;
        tracking = 0.8f;
        visionRange = 100f;
        base.Reset();
    }

    //pushes player away and briefly disables input
    private void Shove(Collider2D other)
    {
        if(other.tag == "Player")
        {
            PlayerClass player = other.GetComponent<PlayerClass>();
            Vector2 dir = (player.floorPosition-floorPosition).normalized;
            PlayerClass.OnStunned.Invoke(SHOVE_DURATION);
            player.rb.AddForce(dir*SHOVE_FORCE);
        }
    }

    public void Hit(Collider2D other, Act act)
    {
        switch(act)
        {
            case Act.SHOVE:
                Shove(other);
                break;
            default:
                Debug.LogError($"Unrecognized Ranger Act {act}");
                break;
        }
    }

    protected override void UpdateAnimator()
    {
        Debug.Log($"Ranger.cs still needs to implement UpdateAnimator()");
    }
}