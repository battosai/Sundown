using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: controls player status

public class PlayerClass : CharacterClass
{
	public static readonly int DAILY_FOOD_REQUIREMENT = 30;

	public static bool isHuman {get; private set;}
	public static bool isFed {get; private set;}
	private int food;

	// Use this for initialization
	void Start()
	{
		base.reset();
		isHuman = true;
		isFed = false;
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

	private void addFood(int value)
	{
		food += value;
		if(food >= DAILY_FOOD_REQUIREMENT)
			isFed = true;
	}
}
