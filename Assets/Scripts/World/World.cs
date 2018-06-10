using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: handles creation of world (spawn critters, interactables, etc)
//NOTE: EACH NODE NEEDS A MESHFILTER AND MESHRENDERER AS WELL AS MATERIAL ADDED

public class World : MonoBehaviour
{
	public static readonly int WORLD_SIZE = GameState.DAYS_TO_WIN;
	public static readonly float NODE_SPACING = MapGenerator.COLS*MeshGenerator.SQUARE_SIZE;
	public static List<GameObject> nodes {get; private set;}
	private GameObject startNode;
	private Transform trans;
	private MapGenerator mapGen;
	private MeshGenerator meshGen;
	private ColliderGenerator collGen;

	void Awake()
	{
		startNode = GameObject.Find("Node0");
		trans = GetComponent<Transform>();
		nodes = new List<GameObject>();
		mapGen = GetComponent<MapGenerator>();
		meshGen = GetComponent<MeshGenerator>();
		collGen = GetComponent<ColliderGenerator>();
	}

	// Use this for initialization
	void Start()
	{
		generateWorldNodes();
		reset();
	}

	// Update is called once per frame
	void Update()
	{
		if(Input.GetMouseButtonDown(0))
			reset();
	}

	private void reset()
	{
		generateMapMeshCollider();
	}

	//populates each worldnode with some wildlife
	private void generateWildlife()
	{
		foreach(GameObject node in nodes)
		{
			break;
		}
	}

	//rolls a new map, mesh, and pool of colliders for walls
	private void generateMapMeshCollider()
	{
		foreach(GameObject node in nodes)
		{
			int[,] map = mapGen.GenerateMap();
			Mesh mesh = meshGen.GenerateMesh(map);
			node.GetComponent<WorldNode>().meshFilter.mesh = mesh;
			collGen.GenerateCollider(node);
		}
	}

	//should only be used once at the start to instantiate the nodes
	private void generateWorldNodes()
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
			node.name = "Node" + i;
			node.GetComponent<WorldNode>().setNodeID(i);
			nodes.Add(node);
			node.GetComponent<Transform>().position = new Vector2(i*NODE_SPACING, 0f);
		}
	}
}
