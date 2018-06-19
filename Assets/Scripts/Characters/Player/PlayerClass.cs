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
	public int food {get; private set;}
	public PlayerInput inputs {get; private set;}
	public PlayerActions actions {get; private set;}
	public Collider2D pushBox {get; private set;}
	private World world;

	public void SetFood(int food){this.food = food;}

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
}
