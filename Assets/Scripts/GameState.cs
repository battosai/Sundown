using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour 
{
	public static readonly int daysToWin = 10;
	public int day {get; private set;}

	// Use this for initialization
	void Start () 
	{
		reset();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	private void reset()
	{
		day = 0;
	}
}
