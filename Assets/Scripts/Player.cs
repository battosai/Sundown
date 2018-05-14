using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: controls player status

public class Player : MonoBehaviour
{
	public static readonly int DAILY_FOOD_REQUIREMENT = 30;
	public static readonly int BASE_SPEED = 10;

	public static int nodeID {get; private set;}
	public static bool isHuman {get; private set;}
	public static bool isFed {get; private set;}
	public static float speed {get; private set;}
 	public static Transform trans;
	public static Rigidbody2D rb;
	public static SpriteRenderer rend;
	public static Collider2D iColl;
	private int food;

	public static void setNodeID(int id){nodeID = id;}

	void Awake()
	{
		trans = GetComponent<Transform>();
		rb = GetComponent<Rigidbody2D>();
		rend = GetComponent<SpriteRenderer>();
		iColl = GameObject.Find("InteractionCollider").GetComponent<Collider2D>();
	}

	// Use this for initialization
	void Start()
	{
		reset();
	}

	// Update is called once per frame
	void Update()
	{
		//check to see if player is clicking at exit to go to next worldnode

	}

	public void toggleDayNight()
	{
		if(GameState.isDaytime)
		{
			isHuman = true;
			isFed = false;
		}
		else
		{
			isHuman = false;
		}
	}

	//will also be used when replaying game
	//place all initializations in here
	public static void reset()
	{
		isHuman = true;
		isFed = false;
		speed = BASE_SPEED;
	}

	private void addFood(int value)
	{
		food += value;
		if(food >= DAILY_FOOD_REQUIREMENT)
			isFed = true;
	}
}
