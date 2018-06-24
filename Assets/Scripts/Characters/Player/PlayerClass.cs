using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: controls player status
//NOTE: https://stackoverflow.com/questions/23535304/getting-the-width-of-a-sprite
//set all objects with sprites to use floorHeight to decide rendering order

public class PlayerClass : CharacterClass
{
	public readonly int DAILY_FOOD_REQUIREMENT = 30;
	public Sprite human, halfHuman, werewolf;
	public bool isHuman {get; private set;}
	public bool isFed {get; private set;}
	public int strength {get; private set;}
	public int food {get; private set;}
	public PlayerInput inputs {get; private set;}
	public PlayerActions actions {get; private set;}
	public Collider2D pushBox {get; private set;}

	public void SetFood(int food){this.food = food;}

	public override void Awake()
	{
		base.Awake();
		inputs = GetComponent<PlayerInput>();
		actions = GetComponent<PlayerActions>();
		pushBox = GetComponent<Collider2D>();
	}

	// Update is called once per frame
	void Update()
	{
	}

	//called by gamestate in masterreset
	public override void Reset()
	{
		base.Reset();
		SetNodeID(0);
		World.nodes[nodeID].SetActive(true);
		Debug.Log(nodeID+" is active");
		trans.position = World.nodes[nodeID].GetComponent<WorldNode>().playerSpawn.transform.position;
		rend.sprite = human;
		isHuman = true;
		isFed = false;
		strength = 5;
		food = 0;
	}

	//called whenever player goes to next node or maybe by will
	public void Shapeshift()
	{
		isHuman = !isHuman;
		if(isHuman)
			rend.sprite = human;
		else
			rend.sprite = werewolf; //will be half human eventually
	}
}
