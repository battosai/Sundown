using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: controls player status
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
	private readonly int FOOD_LOSS = 10;
	private readonly int HUNGER_RATE = 5;
	private readonly int HUMAN_THRESHOLD = 10;
	private float time;

	public void SetFood(int food)
	{
		this.food = food;
		time = Time.time;
	}

	public override void Awake()
	{
		base.Awake();
		inputs = GetComponent<PlayerInput>();
		actions = GetComponent<PlayerActions>();
		pushBox = GetComponent<Collider2D>();
	}

	public void Start()
	{
		setFloorHeight();
	}

	// Update is called once per frame
	void Update()
	{
		setFloorHeight();
		hungerHandler();
	}

	//called by gamestate in masterreset
	public override void Reset()
	{
		base.Reset();
		SetNodeID(0);
		World.nodes[nodeID].SetActive(true);
		trans.position = SetFloorPosition(World.nodes[nodeID].GetComponent<WorldNode>().playerSpawn.transform.position);
		rend.sprite = human;
		isHuman = true;
		isFed = false;
		strength = 5;
		food = 100;
		time = Time.time;
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

	private void hungerHandler()
	{
		if(Time.time-time > HUNGER_RATE)
		{
			time = Time.time;
			SetFood(Mathf.Max(food-FOOD_LOSS, 0));
			Debug.Log("Player has "+food+" food!");
		}
		if(food == 0 && isHuman)
		{
			Debug.Log("Becoming a werewolf!");
			Shapeshift();
		}
		else if(food > HUMAN_THRESHOLD && !isHuman)
		{
			Debug.Log("Returning to human!");
			Shapeshift();
		}
	}
	
}
