using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: handles creation of world (spawn critters, interactables, etc)
//USEFUL LINKS: https://unity3d.com/learn/tutorials/s/procedural-cave-generation-tutorial

public class World : MonoBehaviour
{
	public static readonly int worldSize = 10;

	private List<WorldNode> world;

	public List<WorldNode> getWorld() {return this.world;}

	void Awake()
	{
	}

	// Use this for initialization
	void Start()
	{
		generate();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void generate()
	{
		//create nodes, figure out what should belong to a node
		for(int nodeID = 0; nodeID < worldSize; nodeID++)
		{
			world.Add(new WorldNode(nodeID));
		}
	}
}
