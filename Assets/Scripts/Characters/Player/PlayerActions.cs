using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class PlayerActions : MonoBehaviour, IHitboxResponder
{
	public bool isAttacking;
	public bool isAttackOnCooldown;
	//Vector2: Offset, Vector2: Size
	//position should default to the right side, use isLeft to flip
	private Vector2[] ATTACK = {new Vector2(10, -5), new Vector2(10, 8)};
	private Vector2[] INTERACT = {new Vector2(0, -12), new Vector2(5, 2)};
	private Vector2[] SHAPESHIFT = {Vector2.zero, new Vector2(20, 20)};
	private PlayerClass player;
	private Hitbox hitbox;

	public void Reset()
	{
		isAttacking = false;
	}

	public void Awake()
	{
		player = GetComponent<PlayerClass>();
		hitbox = GetComponent<Hitbox>();
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
			StartCoroutine(dash(player.input.GetMousePos()));
	}

	private IEnumerator dash(Vector2 target)
	{
		float DASH = 40f;
	 	float DASH_TIME = 0.2f;
		float startTime = Time.time;
		while(Time.time-startTime < DASH_TIME)
		{
			player.rb.velocity = PathFinding.GetVelocity(player.floorPosition, target, DASH);
			yield return null;
		}
	}

	public void Hit(Collider2D other, Act act)
	{
		switch(act)
		{
			case Act.INTERACT:
				interact(other);
				break;
			case Act.ATTACK:
				attack(other);
				break;
			case Act.SHAPESHIFT:
				shapeshift(other);
				break;
			default:
				Debug.LogError($"Unknown Player Act {act}");
				break;
		}
	}

	public void AttackCheck(int attackCount)
	{
		int dir = player.isLeft ? -1 : 1;
		hitbox.mask.useTriggers = false;
		hitbox.SetAct(Act.ATTACK);
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
		hitbox.SetAct(Act.INTERACT);
		hitbox.SetOffset(INTERACT[0]);
		hitbox.SetSize(INTERACT[1]);
		hitbox.StartCheckingCollision();
		hitbox.CheckCollision();
		hitbox.StopCheckingCollision();
	}

	public void ShapeshiftCheck()
	{
		hitbox.mask.useTriggers = false;
		hitbox.SetAct(Act.SHAPESHIFT);
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
		if(other.tag == "NPC" || other.tag == "Hero")
		{
			CharacterClass npc = other.GetComponent<CharacterClass>();
			// npc.SetAlarmPoint(player.floorPosition);
			npc.SetIsAlarmed(true);
		}
	}

	private void attack(Collider2D other)
	{
		if(other.tag == "NPC" || other.tag == "Wildlife" || other.tag == "Hero")
		{
			CharacterClass character = other.GetComponent<CharacterClass>();
			// character.SetAlarmPoint(player.floorPosition);
			character.SetIsAlarmed(true);
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
				player.gameState.NodeTransition(player.nodeID); //call before changing player's current nodeId
				return;
			case "BuildingEntrance":
				Building building = other.transform.parent.GetComponent<Building>();
				World.activeBuilding.SetActive(true);
				World.nodes[player.nodeID].SetActive(false);
				player.gameState.GetHero().gameObject.SetActive(false);
				building.Load(player);
				return;
			case "BuildingExit":
				Interior interior = other.transform.parent.GetComponent<Interior>();
				interior.building.Store(player);
				World.nodes[player.nodeID].SetActive(true);
				World.activeBuilding.SetActive(false);
				player.gameState.GetHero().gameObject.SetActive(true);
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
