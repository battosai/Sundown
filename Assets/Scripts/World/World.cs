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
			List<Vector2> points = new List<Vector2>();
			//get list of valid wildlife spawn points
			for(int i = 0; i < Random.Range(1, 10); i++)
			{
				Vector2 point = getValidPoint(node);
				points.Add(point);	
			}
			//use wildlife object pool or spawn new ones
			for(int i = 0; i < wnode.wildlifePool.Count; i++)
			{
				if(points.Count == 0)
				{
					for(int j = i; j < wnode.wildlifePool.Count; j++)
						wnode.wildlifePool[j].SetActive(false);
					break;
				}
				Vector2 point = points[0];
				wnode.wildlifePool[i].transform.position = point;
				wnode.wildlifePool[i].SetActive(true);
				points.Remove(point);
			}
			for(int i = 0; i < points.Count; i++)
			{
				Vector2 point = points[i];
				GameObject animal = Instantiate(wildlifePrefabs[0], node.transform.Find("Wildlife"));
				//do below when pool works for any wildlife prefab, right now will not change sprite
				// GameObject animal = Instantiate(wildlifePrefabs[Random.Range(0, wildlifePrefabs.Count)], node.transform.Find("Wildlife"));
				animal.transform.position = point;
				wnode.AddPoolObject(animal, wnode.wildlifePool);
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

	//returns a point in the node that is not a wall
	private Vector2 getValidPoint(GameObject node)
	{
		WorldNode wnode = node.GetComponent<WorldNode>();
		while(true)
		{
			float mapWidth = MapGenerator.COLS*MeshGenerator.SQUARE_SIZE;
			float mapHeight = MapGenerator.ROWS*MeshGenerator.SQUARE_SIZE;
			int row = Random.Range(1, MapGenerator.COLS-2);
			int col = Random.Range(1, MapGenerator.ROWS-2);
			if(wnode.map[row, col] == MapGenerator.FLOOR)
			{
				float x = node.transform.position.x-mapWidth/2+col*MeshGenerator.SQUARE_SIZE+MeshGenerator.SQUARE_SIZE/2;
				float y = node.transform.position.y-mapHeight/2+row*MeshGenerator.SQUARE_SIZE+MeshGenerator.SQUARE_SIZE/2;
				Debug.DrawLine(new Vector2(x-0.5f, y), new Vector2(x+0.5f, y), Color.cyan, 100f);
				return new Vector2(x, y); 
			}
		}
	}
}
