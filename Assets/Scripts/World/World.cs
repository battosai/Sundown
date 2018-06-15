using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: handles creation of world (spawn critters, interactables, etc)
//NOTE: EACH NODE NEEDS A MESHFILTER AND MESHRENDERER AS WELL AS MATERIAL ADDED

public class World : MonoBehaviour
{
	public List<GameObject> wildlifePrefabs;

	public static readonly int WORLD_SIZE = GameState.DAYS_TO_WIN;
	public static readonly float NODE_SPACING = MapGenerator.COLS*MeshGenerator.SQUARE_SIZE;
	public static List<GameObject> nodes {get; private set;}
	private GameObject startNode;
	private Transform trans;
	private MapGenerator mapGen;
	private MeshGenerator meshGen;
	private ColliderGenerator collGen;

	public void Awake()
	{
		startNode = GameObject.Find("Node0");
		trans = GetComponent<Transform>();
		nodes = new List<GameObject>();
		mapGen = GetComponent<MapGenerator>();
		meshGen = GetComponent<MeshGenerator>();
		collGen = GetComponent<ColliderGenerator>();
	}
	
	public void Start()
	{

	}

	//called by GameState in masterreset
	public void Reset()
	{
		if(nodes.Count == 0)
			generateWorldNodes();
		generateMapMeshCollider();
		generateWildlife();
		foreach(GameObject node in nodes)
			node.GetComponent<WorldNode>().ParentReset();
	}

	//populates each worldnode with some wildlife
	private void generateWildlife()
	{
		foreach(GameObject node in nodes)
		{
			WorldNode wnode = node.GetComponent<WorldNode>();
			int wildlifePoolSize = wnode.wildlifePool.Count;
			int wildlifeCount = Random.Range(1, 10);
			Debug.Log("adding "+wildlifeCount+" deer");
			for(int i = 0; i < wildlifeCount; i++)
			{
				float mapWidth = MapGenerator.COLS*MeshGenerator.SQUARE_SIZE;
				float mapHeight = MapGenerator.ROWS*MeshGenerator.SQUARE_SIZE;
				int row = Random.Range(1, MapGenerator.COLS-2);
				int col = Random.Range(1, MapGenerator.ROWS-2);
				float x = node.transform.position.x-mapWidth/2+col*MeshGenerator.SQUARE_SIZE+MeshGenerator.SQUARE_SIZE/2;
				float y = node.transform.position.y-mapHeight/2+row*MeshGenerator.SQUARE_SIZE+MeshGenerator.SQUARE_SIZE/2;
				Vector2 point = new Vector2(x, y); 
				if(wnode.map[row, col] == MapGenerator.FLOOR)
				{
					if(wildlifePoolSize == 0)
					{
						GameObject animal = Instantiate(wildlifePrefabs[1], node.transform.Find("Wildlife"));	
						animal.transform.position = point;
						wnode.AddWildlifePool(animal);
						continue;
					}
					wnode.wildlifePool[0].transform.position = point;
					wnode.wildlifePool[0].active = true;
					wildlifePoolSize -= 1;
					continue;
				}
			}
			for(int i = 0; i < wildlifePoolSize-wildlifeCount; i++)
			{
				wnode.wildlifePool[i+wildlifeCount-1].active = false;
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
