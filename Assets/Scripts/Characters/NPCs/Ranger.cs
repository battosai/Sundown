using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : HeroClass, IHitboxResponder
{
    private readonly int MASTERY = 2;
    private readonly float AGGRO_RANGE = 100f;

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
       hitBox.SetResponder(this); 
    }

    public void Update()
    {
        if(isPresent)
        {
            if(playerSpotted())
            {
                Debug.Log("YOU'VE BEEN SPOTTED!");
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

    private void interact(Collider2D other)
    {
        
    }
}