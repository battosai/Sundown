using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: handles player controls and input

public class Player : MonoBehaviour
{
	public static readonly int DAILY_FOOD_REQUIREMENT = 30;

	public static int nodeID {get; private set;}
	public static bool isHuman {get; private set;}
	public static bool isFed {get; private set;}

	private Transform trans;
	private SpriteRenderer rend;
	private int food;

	void Awake()
	{
		trans = GetComponent<Transform>();
		rend = GetComponent<SpriteRenderer>();
	}

	// Use this for initialization
	void Start()
	{
		reset();
	}

	// Update is called once per frame
	void Update()
	{
	}

	//called by gamestate.checkTimeLimit
	public static void toggleDayNight(bool isDaytime)
	{
		if(isDaytime)
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
	}

	private void addFood(int value)
	{
		food += value;
		if(food >= DAILY_FOOD_REQUIREMENT)
			isFed = true;
	}
}
