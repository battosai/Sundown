using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: handles creation of world (spawn critters, interactables, etc)
//USEFUL LINKS: https://unity3d.com/learn/tutorials/s/procedural-cave-generation-tutorial

public class World : MonoBehaviour
{
	public static readonly int WORLD_SIZE = GameState.DAYS_TO_WIN;
	public static List<WorldNode> world {get; private set;}

	void Awake()
	{
	}

	// Use this for initialization
	void Start()
	{
		reset();
		generateWorld();
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void generateWorld()
	{
		world.Clear();
		for(int nodeID = 0; nodeID < WORLD_SIZE; nodeID++)
		{
			world.Add(new WorldNode(nodeID));
		}
	}

	public static void reset()
	{
		isDaytime = true;
	}
}
