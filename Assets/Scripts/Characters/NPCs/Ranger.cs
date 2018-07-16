using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : HeroClass
{
    private readonly int MASTERY = 2;

	public override void Awake()
	{
		base.Awake();
        gameState.SetHero(this);
		player = GameObject.Find("Player").GetComponent<PlayerClass>();
        pushBox = GetComponent<Collider2D>();
	}

    public void Update()
    {
        setFloorHeight();
    }

    public override void Track(int nodeID)
    {
        Debug.Log("Ranger Mastery Track!"); 
        if(tracking >= PLAYER_FOUND)
        {
            Debug.Log("Ranger has found your current location!");
            presentInNode(true, nodeID+1);
            SetLead(0);
        }
        else
        {
            presentInNode(false);
            WorldNode wnode = World.nodes[nodeID].GetComponent<WorldNode>();
            SetLead(lead+wnode.clues*tracking*MASTERY);
        }
        Debug.Log("Ranger now has "+lead+" leads");
    }

    public override void Reset()
    {
        base.Reset();
        SetTracking(0.8f); 
        SetLead(0f);
        presentInNode(false);
    }
}