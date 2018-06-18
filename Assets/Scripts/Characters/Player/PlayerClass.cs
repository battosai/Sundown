using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: controls player status

public class PlayerClass : CharacterClass
{
	public readonly int DAILY_FOOD_REQUIREMENT = 30;
	public bool isHuman {get; private set;}
	public bool isFed {get; private set;}
	public int strength {get; private set;}
	public PlayerInput inputs {get; private set;}
	public PlayerActions actions {get; private set;}
	public Collider2D pushBox {get; private set;}
	private int food;
	private World world;

	public override void Awake()
	{
		base.Awake();
		inputs = GetComponent<PlayerInput>();
		actions = GetComponent<PlayerActions>();
		pushBox = GetComponent<Collider2D>();
		world = GameObject.Find("World").GetComponent<World>();
	}

	// Update is called once per frame
	void Update()
	{
		//check to see if player is clicking at exit to go to next worldnode

	}

	//called by gamestate in masterreset
	public override void Reset()
	{
		base.Reset();
		base.SetNodeID(0);
		trans.position = World.nodes[nodeID].GetComponent<WorldNode>().playerSpawn.transform.position;
		isHuman = true;
		isFed = false;
		strength = 5;
	}

	public void AddFood(int value)
	{
		food += value;
		if(food >= DAILY_FOOD_REQUIREMENT)
			isFed = true;
		Debug.Log("Player has "+food+" food!");
	}
}
