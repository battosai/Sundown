using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroClass : CharacterClass
{
	public float tracking {get; private set;}//used to determine how likely a clue will be discovered
	public float lead {get; private set;}
	protected float leash = 20f;
	protected readonly int PLAYER_FOUND = 15;
	protected PlayerClass player;
	public void SetTracking(float tracking){this.tracking=tracking;}
	public void SetLead(float lead){this.lead=lead;}

	public virtual void Track(int nodeID)
	{
		Debug.Log("Default Track!");
 		if(tracking >= PLAYER_FOUND)
        {
            SetNodeID(nodeID+1);
            SetLead(0);
        }
        else
        {
            WorldNode wnode = World.nodes[nodeID].GetComponent<WorldNode>();
            SetLead(lead+wnode.clues*tracking);
        }
	}

	// private void follow()
	// {
	// 	if(nodeID != player.nodeID)
	// 		return;
	// 	float distance = Vector2.Distance(player.trans.position, trans.position);
	// 	if(distance > leash)
	// 	{
	// 		rb.velocity = getVelocityTowardPlayer();
	// 	}
	// 	else
	// 	{
	// 		rb.velocity = Vector2.zero;
	// 	}
	// }

	// private Vector2 getVelocityTowardPlayer()
	// {
	// 	float distance;
	// 	Vector2 direction;
	// 	Vector2 velocity;
	// 	distance = Vector2.Distance(player.trans.position, trans.position);
	// 	direction = player.trans.position - trans.position;
	// 	velocity = new Vector2((direction.x*speed)/distance, (direction.y*speed)/distance);
	// 	return velocity;
	// }
}
