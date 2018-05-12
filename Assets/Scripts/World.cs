using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: handles creation of world (spawn critters, interactables, etc)
//USEFUL LINKS: https://unity3d.com/learn/tutorials/s/procedural-cave-generation-tutorial

public class World : MonoBehaviour
{
	public static readonly int WORLD_SIZE = GameState.DAYS_TO_WIN;
	public static readonly float NODE_SPACING = 100f;
	public static List<GameObject> nodes {get; private set;}
	private GameObject startNode;
	private Transform trans;

	void Awake()
	{
		startNode = GameObject.Find("Node");
		trans = GetComponent<Transform>();
		nodes = new List<GameObject>();
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

	private void generateWorld()
	{
		if(nodes == null)
		{
			Debug.Log("Error: nodes is null");
			return;
		}
		nodes.Clear();
		startNode.GetComponent<WorldNode>().setNodeID(nodes.Count);
		nodes.Add(startNode);
		for(int i = 1; i < WORLD_SIZE; i++)
		{
			GameObject node = Instantiate(startNode, trans);
			node.GetComponent<WorldNode>().setNodeID(i);
			nodes.Add(node);
			node.GetComponent<Transform>().position = new Vector2(i*NODE_SPACING, i*NODE_SPACING);
		}
	}

	private void reset()
	{
		generateWorld();
	}
}
