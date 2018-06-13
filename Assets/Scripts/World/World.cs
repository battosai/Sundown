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

	// Update is called once per frame
	void Update()
	{
	}

	//called by GameState in masterreset
	public void Reset()
	{
		if(nodes.Count == 0)
			generateWorldNodes();
		generateMapMeshCollider();
		generateBuildings();
		foreach(GameObject node in nodes)
			node.GetComponent<WorldNode>().ParentReset();
	}

	//populates each worldnode with some wildlife
	private void generateWildlife()
	{
		foreach(GameObject node in nodes)
		{
			break;
		}
	}
	
	//currently trying to see if a point is a wall or not
	private void generateBuildings()
	{
		foreach(GameObject node in nodes)
		{
			WorldNode wnode = node.GetComponent<WorldNode>();
			int row = Random.Range(0, MapGenerator.ROWS-1);
			int col = Random.Range(0, MapGenerator.COLS-1);	
			Vector2 point = new Vector2(wnode.nodeID*NODE_SPACING+col*MeshGenerator.SQUARE_SIZE+MeshGenerator.SQUARE_SIZE/2, row*MeshGenerator.SQUARE_SIZE+MeshGenerator.SQUARE_SIZE/2);
			if(wnode.map[row, col] == 1)
			{
				Debug.Log("WALL");
				Debug.DrawLine(new Vector2(point.x-5f, point.y), new Vector2(point.x+5f, point.y), Color.red, 100f);
			}
			else
			{
				Debug.Log("OPEN");
				Debug.DrawLine(new Vector2(point.x-5f, point.y), new Vector2(point.x+5f, point.y), Color.green, 100f);
			}
		}
	}

	//rolls a new map, mesh, and pool of colliders for walls
	private void generateMapMeshCollider()
	{
		foreach(GameObject node in nodes)
		{
			WorldNode wnode = node.GetComponent<WorldNode>();
			int[,] map = mapGen.GenerateMap();
			wnode.SetMap(map);
			Mesh mesh = meshGen.GenerateMesh(map);
			wnode.meshFilter.mesh = mesh;
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
		startNode.GetComponent<WorldNode>().SetNodeID(nodes.Count);
		nodes.Add(startNode);
		for(int i = 1; i < WORLD_SIZE; i++)
		{
			GameObject node = Instantiate(startNode, trans);
			node.name = "Node" + i;
			node.GetComponent<WorldNode>().SetNodeID(i);
			nodes.Add(node);
			node.GetComponent<Transform>().position = new Vector2(i*NODE_SPACING, 0f);
		}
	}
}
