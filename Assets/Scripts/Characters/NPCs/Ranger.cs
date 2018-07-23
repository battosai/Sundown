using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : HeroClass, IHitboxResponder
{
    private readonly int MASTERY = 2;
    private readonly float AGGRO_RANGE = 100f;
    private enum State {MOVING, INSPECTING, IDLE, CHASING};
    private State state;
	private Vector2 INTERACT_SIZE = new Vector2(50f, 50f);
    private List<GameObject> usedLeads;
    private float time;
    private GameObject target;

	public override void Awake()
	{
		base.Awake();
        gameState.SetHero(this);
		player = GameObject.Find("Player").GetComponent<PlayerClass>();
        hitBox = GetComponent<Hitbox>();
        pushBox = GetComponent<Collider2D>();
	}

    public void Start()
    {
        init();
        hitBox.SetResponder(this); 
    //    usedLeads = new List<GameObject>();
    }

    public void Update()
    {
        if(isPresent)
        {
            if(playerSpotted())
            {
                Debug.Log("YOU'VE BEEN SPOTTED!");
            }
            switch(state)
            {
                case State.IDLE:
                    break;
                case State.MOVING:
                    break;
                case State.INSPECTING:
                    interactCheck();
                    break;
                default:
                    Debug.Log("[Error] Unknown Ranger Action: "+state);
                    break;
            }
        }
        setFloorHeight();
    }

    public override void Track(int nodeID)
    {
        Debug.Log("Ranger Mastery Track!"); 
        WorldNode wnode = World.nodes[nodeID].GetComponent<WorldNode>();
        SetLead(lead+wnode.clues*tracking*MASTERY);
        presentInNode(false);
        if(lead >= PLAYER_FOUND)
        {
            Debug.Log("Ranger has found your current location!");
            presentInNode(true, nodeID+1);
            SetLead(0);
        }
        Debug.Log("Ranger now has "+lead+" leads");
    }

    public override void Reset()
    {
        base.Reset();
        SetLead(0f);
        presentInNode(false);
        tracking = 0.8f;
        visionRange = 100f;
        state = State.IDLE;
        time = Time.time;
        usedLeads = new List<GameObject>();
    }

    public void Hit(Collider2D other, Action action)
    {
        switch(action)
        {
            case Action.INTERACT:
                interact(other);
                break;
            case Action.ATTACK:
                break;
            default:
                Debug.Log("[Error] Unrecognized Ranger Action");
                break;
        }
    }

    private void interactCheck()
    {
        hitBox.mask.useTriggers = false;
		hitBox.SetAction(Action.INTERACT);
		hitBox.SetOffset(floorPosition);
		hitBox.SetSize(INTERACT_SIZE);
		hitBox.StartCheckingCollision();
		hitBox.CheckCollision();
		hitBox.StopCheckingCollision();
    }

    //effectively the ranger's "scout" ability
    private void interact(Collider2D other)
    {
        target = null;
        GameObject obj = other.gameObject;
        if(!usedLeads.Contains(obj))
        {
            CharacterClass character = obj.GetComponent<CharacterClass>();
            if(character != null)
            {
               switch(character.type)
               {
                    case CharacterType.PLAYER:
                        break;
                    case CharacterType.WILDLIFE:
                        Wildlife animal = obj.GetComponent<Wildlife>();
                        if(animal.isDead)
                            target = obj;
                        break;
                    case CharacterType.TOWNSPERSON:
                        break;
                    default:
                        Debug.Log("[Error] "+obj.name+" has no CharacterType");
                        break;
               } 
               if(target != null)
               {
                    usedLeads.Add(obj);
               }
            }
        }
        state = State.MOVING;
    }
}