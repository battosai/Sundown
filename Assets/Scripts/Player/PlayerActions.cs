using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Action {NONE, TRAVEL, ATTACK};
public class PlayerActions : MonoBehaviour, IHitboxResponder
{
	private readonly Vector2 A_OFFSET = new Vector2(1, 0); //use isLeft to flip
	private readonly Vector2 T_OFFSET = new Vector2(0, -12);
	private readonly Vector2 T_SIZE = new Vector2(5, 2);
	private PlayerClass player;
	private Hitbox hitbox;

	void Awake()
	{
		player = GetComponent<PlayerClass>();
		hitbox = GetComponent<Hitbox>();
	}

	// Use this for initialization
	void Start()
	{
		hitbox.setResponder(this);
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void collisionWith(Collider2D other, Action action)
	{
		switch(action)
		{
			case Action.TRAVEL:
				travel();
				break;
			case Action.ATTACK:
				Debug.Log("implement victim's hurtbox gethitby method");
				break;
			case Action.NONE:
			default:
				break;
		}
	}

	public void attack()
	{
		Debug.Log("Lunge!");
		hitbox.setAction(Action.ATTACK);
	}

	//triggers hitbox to check for travel colliders
	public void travelCheck()
	{
		hitbox.setAction(Action.TRAVEL);
		hitbox.setOffset(T_OFFSET);
		hitbox.setSize(T_SIZE);
		hitbox.startCheckingCollision();
		hitbox.checkCollision();
		hitbox.stopCheckingCollision();
	}

	//implement the actual functionality of traveling
	private void travel()
	{
		Debug.Log("Traveling!");
		player.setNodeID(player.nodeID+1);
		GameObject node = World.nodes[player.nodeID];
		GameObject spawn = node.GetComponent<WorldNode>().playerSpawn;
		player.trans.position = spawn.transform.position;
	}
}
