using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public abstract class HeroClass : CharacterClass
{
	protected readonly int PLAYER_FOUND = 15;
    protected enum State {IDLE, INSPECT, ATTACK, COROUTINE};
    protected State state;
	protected float leads;
	protected float tracking;
	protected float visionRange;
	protected bool isPresent;
	public bool isArenaTime {get; protected set;}
	protected PlayerClass player;
	protected Hitbox hitBox;

    protected abstract void InitializeStateMachine();

	//common one time setups
	protected void init()
	{
		SetType(CharacterType.HERO);
	}

	public virtual void ArenaUpdate()
	{
		Debug.Log("DEFAULT ARENA UPDATE!");
	}

	public virtual void Track(int nodeID)
	{
		Debug.Log("Default Track!");
 		if(leads >= PLAYER_FOUND)
        {
            SetNodeID(nodeID+1);
			leads = 0;
        }
        else
        {
            WorldNode wnode = World.wnodes[nodeID];
			leads += wnode.clues*tracking;
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
					return true;
			}
        }
        return false;
    }

	public void PlaceInArena()
	{
		isPresent = true;
		isArenaTime = true;
		rend.enabled = true;
		pushBox.enabled = true;
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
				Debug.LogError("Invalid NodeID when making Hero present in node");
				return;
			}
			nodeMap = PathFinding.Node.MakeNodeMap(World.wnodes[nodeID].map, nodeID);
			Vector2 spawn = world.GetValidPoints(nodeID, 1)[0];
			trans.position = spawn;
		}
	}

	protected new IEnumerator takePath(Vector2 destination, System.Action callback=null)
	{
		Debug.Log("Taking path!");
		float tolerance = 1f;
		List<Vector2> path = PathFinding.AStarJump(floorPosition, destination, nodeMap, nodeID);
		Debug.DrawLine(new Vector3(destination.x-1f, destination.y, 0f), new Vector3(destination.x+1f, destination.y, 0f), Color.red, 100f);
		for(int i = 0; i < path.Count; i++)
		{
			if(health <= 0)
				break;
			else if(playerSpotted())
			{
				callback();
				break;
			}
			while(true)
			{
				if(health <= 0)
					break;
				if(Vector2.Distance(floorPosition, path[i]) <= tolerance)
					break;
				Debug.DrawLine(new Vector3(path[i].x-1f, path[i].y, 0f), new Vector3(path[i].x+1f, path[i].y, 0f), Color.cyan, 1f);
				rb.velocity = PathFinding.GetVelocity(floorPosition, path[i], speed);
				yield return null;
			}
		}
		Debug.Log("Reached end of path");
		if(callback != null)
			callback();
	}

	public override void Reset()
	{
        state = State.IDLE;
		SetHealth(maxHealth);
		base.Reset();
	}
}
