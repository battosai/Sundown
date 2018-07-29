using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroClass : CharacterClass
{
	public float lead {get; private set;}
	protected readonly int PLAYER_FOUND = 15;
	protected float tracking;
	protected float visionRange;
	protected bool isPresent;
	protected PlayerClass player;
	protected Hitbox hitBox;
	protected Collider2D pushBox;
	public void SetLead(float lead){this.lead=lead;}

	//common one time setups
	protected void init()
	{
		SetType(CharacterType.HERO);
	}

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
            WorldNode wnode = World.wnodes[nodeID];
            SetLead(lead+wnode.clues*tracking);
        }
	}

	//is the player in the ranger's line of sight
    protected bool playerSpotted()
    {
        Vector2 direction = player.floorPosition-floorPosition;
        if(isLeft && Mathf.Sign(direction.x) < 0 || !isLeft && Mathf.Sign(direction.x) > 0)
        {
            RaycastHit2D vision = Physics2D.Raycast(floorPosition, player.floorPosition-floorPosition, visionRange);
			if(vision.collider != null)
			{
				if(vision.collider.gameObject.name == "Player")
				{
					return true;
				}
			}
        }
        return false;
    }

	//handles making the hero (not) present in the player's node, but not making the game object inactive
	protected void presentInNode(bool isPresent, int nodeID=-1)
	{
		this.isPresent = isPresent;
		rend.enabled = isPresent;
		pushBox.enabled = isPresent;
		SetNodeID(nodeID);
		if(isPresent)
		{
			if(nodeID < 0)
			{
				Debug.Log("[Error] Invalid NodeID when making Hero present in node");
				return;
			}
			nodeMap = PathFinding.Node.MakeNodeMap(player.trans.position, World.wnodes[nodeID].map, nodeID);
			Vector2 spawn = world.GetValidPoint(nodeID);
			trans.position = spawn;
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
