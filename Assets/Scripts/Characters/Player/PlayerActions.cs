using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour, IHitboxResponder
{
	//Vector2: Offset, Vector2: Size
	//position should default to the right side, use isLeft to flip
	private Vector2[] ATTACK = {new Vector2(10, -5), new Vector2(10, 8)};
	private Vector2[] INTERACT = {new Vector2(0, -12), new Vector2(5, 2)};
	private Vector2[] SHAPESHIFT = {Vector2.zero, new Vector2(20, 20)};
	private readonly float DASH = 500f;
	private PlayerClass player;
	private PlayerInput input;
	private Hitbox hitbox;
	private World world;
	private bool isAttacking;

	public void Reset()
	{
		isAttacking = false;
	}

	public void Awake()
	{
		player = GetComponent<PlayerClass>();
		input = GetComponent<PlayerInput>();
		hitbox = GetComponent<Hitbox>();
		world = GameObject.Find("World").GetComponent<World>();
	}

	// Use this for initialization
	public void Start()
	{
		hitbox.SetResponder(this);
	}

	// Update is called once per frame
	public void FixedUpdate()
	{
		if(isAttacking)
		{
			isAttacking = false;
			Vector2 force = PathFinding.GetVelocity(player.floorPosition, player.input.GetMousePos(), DASH);
			player.rb.velocity = Vector2.zero;
			player.rb.AddForce(force, ForceMode2D.Impulse);
		}
	}

	public void Hit(Collider2D other, Action action)
	{
		switch(action)
		{
			case Action.INTERACT:
				interact(other);
				break;
			case Action.ATTACK:
				attack(other);
				break;
			case Action.SHAPESHIFT:
				shapeshift(other);
				break;
			default:
				Debug.Log("[Error] Unknown Player Action");
				break;
		}
	}

	public void AttackCheck()
	{
		isAttacking = true;
		int dir = player.isLeft ? -1 : 1;
		hitbox.mask.useTriggers = false;
		hitbox.SetAction(Action.ATTACK);
		hitbox.SetOffset(new Vector2(ATTACK[0].x*dir, ATTACK[0].y));
		hitbox.SetSize(ATTACK[1]);
		hitbox.StartCheckingCollision();
		hitbox.CheckCollision();
		hitbox.StopCheckingCollision();
	}

	//triggers hitbox to check for travel colliders
	public void InteractCheck()
	{
		hitbox.mask.useTriggers = true;
		hitbox.SetAction(Action.INTERACT);
		hitbox.SetOffset(INTERACT[0]);
		hitbox.SetSize(INTERACT[1]);
		hitbox.StartCheckingCollision();
		hitbox.CheckCollision();
		hitbox.StopCheckingCollision();
	}

	public void ShapeshiftCheck()
	{
		hitbox.mask.useTriggers = false;
		hitbox.SetAction(Action.SHAPESHIFT);
		hitbox.SetOffset(SHAPESHIFT[0]);
		hitbox.SetSize(SHAPESHIFT[1]);
		hitbox.StartCheckingCollision();
		hitbox.CheckCollision();
		hitbox.StopCheckingCollision();
	}

	private void shapeshift(Collider2D other)
	{
	//fnc handles how shapeshift affects other characters
	//playerclass has player shapeshift fnc
		if(other.tag == "NPC")
		{
			CharacterClass npc = other.GetComponent<CharacterClass>();
			npc.SetAlarmPoint(player.floorPosition);
			npc.SetIsAlarmed(true);
		}
	}

	private void attack(Collider2D other)
	{
		if(other.tag == "NPC")
		{
			CharacterClass npc = other.GetComponent<CharacterClass>();
			npc.SetAlarmPoint(player.floorPosition);
			npc.SetIsAlarmed(true);
			Hurtbox hurtbox = other.GetComponent<Hurtbox>();	
			hurtbox.Hurt(player.strength);
		}
	}

	private void interact(Collider2D other)
	{
		//use tag instead of check for null, get rid of getcomponent call, make tags for all these
		switch(other.gameObject.name)
		{
			case "PlayerExit":
				World.nodes[player.nodeID].SetActive(false);
				player.gameState.NodeTransition(player.nodeID); //call before changing player's current nodeId
				player.SetNodeID(player.nodeID+1);
				World.nodes[player.nodeID].SetActive(true);
				GameObject node = World.nodes[player.nodeID];
				GameObject spawn = node.GetComponent<WorldNode>().playerSpawn;
				player.trans.position = player.SetFloorPosition(spawn.transform.position);
				return;
			case "BuildingEntrance":
				Building building = other.transform.parent.GetComponent<Building>();
				World.activeBuilding.SetActive(true);
				World.nodes[player.nodeID].SetActive(false);
				building.Load(player);
				return;
			case "BuildingExit":
				Interior interior = other.transform.parent.GetComponent<Interior>();
				interior.building.Store(player);
				World.nodes[player.nodeID].SetActive(true);
				World.activeBuilding.SetActive(false);
				return;	
			default:
				break;
		}
		Container container = other.gameObject.GetComponent<Container>();
		if(container != null)
		{
			if(!container.isEmpty)
				container.Search();
			else
				Debug.Log("Container is empty!");
		}
		else
		{
			Debug.Log(other.gameObject.name+" is not a container");
		}
	}
}
