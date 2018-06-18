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

	void Awake()
	{
		player = GetComponent<PlayerClass>();
		input = GetComponent<PlayerInput>();
		hitbox = GetComponent<Hitbox>();
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
				if(other.gameObject.name == "PlayerExit")
					travel();
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
		Debug.Log("checking for attack");
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
		Debug.Log("checking for travel");
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
		Debug.Log("You just got slapped for 5hp bitch!");
		Hurtbox hurtbox = other.GetComponent<Hurtbox>();	
		Wildlife wildlife = other.GetComponent<Wildlife>();	
		hurtbox.Hurt(player.strength);
		if(wildlife != null)
			player.AddFood(wildlife.nutrition);
	}

	//implement the actual functionality of traveling
	private void travel()
	{
		Debug.Log("Traveling!");
		player.SetNodeID(player.nodeID+1);
		GameObject node = World.nodes[player.nodeID];
		GameObject spawn = node.GetComponent<WorldNode>().playerSpawn;
		player.trans.position = spawn.transform.position;
	}
}
