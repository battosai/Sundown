using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: manages when game is ready to be progressed, or can't progress

public class GameState : MonoBehaviour
{
	public static readonly int DAY_LENGTH = 60;
	public static readonly int NIGHT_LENGTH = 60;
	public static readonly int DAYS_TO_WIN = 10;
	public static int day {get; private set;}
	public static bool isDaytime {get; private set;}

	private readonly float UNSET_TIME = -1f;
	private Player player;
	private float startTime;

	void Awake()
	{
	}

	// Use this for initialization
	void Start()
	{
		reset();
	}

	// Update is called once per frame
	void Update()
	{
		checkTimeLimit()
	}

	//switches between daytime and nighttime according to their durations
	private void checkTimeLimit()
	{
		if(startTime == UNSET_TIME)
			startTime = Time.time;
		if(isDaytime)
		{
			if(Time.time - startTime >= DAY_LENGTH)
			{
				isDaytime = false;
				startTime = UNSET_TIME;
			}
		}
		else
		{
			if(Time.time - startTime >= NIGHT_LENGTH)
			{
				isDaytime = true;
				startTime = UNSET_TIME;
			}
		}
	}

	private void reset()
	{
		day = 0;
		isDaytime = true;
	}
}
