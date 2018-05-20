using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroClass : CharacterClass
{
	private float leash = 20f;
	private PlayerClass player;

	public override void Awake()
	{
		base.Awake();
		player = GameObject.Find("Player").GetComponent<PlayerClass>();
	}
	// Use this for initialization
	void Start ()
	{
		base.reset();
		base.setNodeID(0);//Random.Range(0, World.WORLD_SIZE-1));
	}

	// Update is called once per frame
	void Update ()
	{
		follow();
	}

	private void follow()
	{
		if(nodeID != player.nodeID)
			return;
		float distance = Vector2.Distance(player.trans.position, trans.position);
		if(distance > leash)
		{
			rb.velocity = getVelocityTowardPlayer();
		}
		else
		{
			rb.velocity = Vector2.zero;
		}
	}

	private Vector2 getVelocityTowardPlayer()
	{
		float distance;
		Vector2 direction;
		Vector2 velocity;
		distance = Vector2.Distance(player.trans.position, trans.position);
		direction = player.trans.position - trans.position;
		velocity = new Vector2((direction.x*speed)/distance, (direction.y*speed)/distance);
		Debug.Log("Speed: " + speed);
		Debug.Log(velocity);
		return velocity;
	}
}
