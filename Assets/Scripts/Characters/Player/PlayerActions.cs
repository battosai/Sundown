using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Action {NONE, TRAVEL, ATTACK};
public class PlayerActions : MonoBehaviour, IHitboxResponder
{
	//Vector2: Position, Vector2: Size
	//position should default to the right side, use isLeft to flip
	private Vector2[] ATTACK = {new Vector2(5, 0), new Vector2(5, 2)};
	private Vector2[] TRAVEL = {new Vector2(0, -12), new Vector2(5, 2)};
	private PlayerClass player;
	private PlayerInput input;
	private Hitbox hitbox;
	private World world;

	void Awake()
	{
		player = GetComponent<PlayerClass>();
		input = GetComponent<PlayerInput>();
		hitbox = GetComponent<Hitbox>();
		world = GameObject.Find("World").GetComponent<World>();
	}

	// Use this for initialization
	void Start()
	{
		hitbox.SetResponder(this);
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void Hit(Collider2D other, Action action)
	{
		switch(action)
		{
			case Action.TRAVEL:
				travel(other);
				break;
			case Action.ATTACK:
				attack(other);
				break;
			case Action.NONE:
			default:
				Debug.Log("Uknown Action!");
				break;
		}
	}

	public void AttackCheck()
	{
		hitbox.mask.useTriggers = false;
		hitbox.SetAction(Action.ATTACK);
		hitbox.SetOffset(ATTACK[0]);
		hitbox.SetSize(ATTACK[1]);
		hitbox.StartCheckingCollision();
		hitbox.CheckCollision();
		hitbox.StopCheckingCollision();
	}

	//triggers hitbox to check for travel colliders
	public void TravelCheck()
	{
		hitbox.mask.useTriggers = true;
		hitbox.SetAction(Action.TRAVEL);
		hitbox.SetOffset(TRAVEL[0]);
		hitbox.SetSize(TRAVEL[1]);
		hitbox.StartCheckingCollision();
		hitbox.CheckCollision();
		hitbox.StopCheckingCollision();
	}

	private void attack(Collider2D other)
	{
		Hurtbox hurtbox = other.GetComponent<Hurtbox>();	
		hurtbox.Hurt(player.strength);
	}

	//implement the actual functionality of traveling
	private void travel(Collider2D other)
	{
		switch(other.gameObject.name)
		{
			case "PlayerExit":
				World.nodes[player.nodeID].SetActive(false);
				player.SetNodeID(player.nodeID+1);
				World.nodes[player.nodeID].SetActive(true);
				GameObject node = World.nodes[player.nodeID];
				GameObject spawn = node.GetComponent<WorldNode>().playerSpawn;
				player.trans.position = player.SetFloorPosition(spawn.transform.position);
				break;
			case "BuildingEntrance":
				Building building = other.transform.parent.GetComponent<Building>();
				World.activeBuilding.SetActive(true);
				World.nodes[player.nodeID].SetActive(false);
				building.Load(player);
				// Debug.Log("Entered building in "+other.transform.parent.parent.parent.gameObject.name);
				break;
			case "BuildingExit":
				Interior interior = other.transform.parent.GetComponent<Interior>();
				interior.building.Store(player);
				World.nodes[player.nodeID].SetActive(true);
				World.activeBuilding.SetActive(false);
				break;	
			default:
				break;
		}
	}
}
