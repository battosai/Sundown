using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: handles creation of world (spawn critters, interactables, etc)

public class World : MonoBehaviour
{
	public List<GameObject> buildingPrefabs;
	public List<GameObject> bigAnimalPrefabs;
	public List<GameObject> smallAnimalPrefabs;
	public Dictionary<string, List<GameObject>> beastiary;

	public static readonly int RESERVED = 2;
	public static readonly int WORLD_SIZE = GameState.DAYS_TO_WIN;
	public static readonly float NODE_SPACING = MapGenerator.COLS*MeshGenerator.SQUARE_SIZE;
	public static List<GameObject> nodes {get; private set;}
	public static GameObject activeBuilding {get; private set;}
	private readonly string SMALL_ANIMAL = "Small";
	private readonly string BIG_ANIMAL = "Big";
	private GameObject startNode;
	private Transform trans;
	private MapGenerator mapGen;
	private MeshGenerator meshGen;
	private ColliderGenerator collGen;

	public void Awake()
	{
		startNode = GameObject.Find("Node0");
		activeBuilding = GameObject.Find("ActiveBuilding");
		trans = GetComponent<Transform>();
		mapGen = GetComponent<MapGenerator>();
		meshGen = GetComponent<MeshGenerator>();
		collGen = GetComponent<ColliderGenerator>();
		nodes = new List<GameObject>();
	}

	public void Start()
	{
		beastiary = new Dictionary<string, List<GameObject>>();
		beastiary[BIG_ANIMAL] = bigAnimalPrefabs;
		beastiary[SMALL_ANIMAL] = smallAnimalPrefabs;
	}
	
	public void DisplayFloor()
	{
		foreach(GameObject node in nodes)
		{
			WorldNode wnode = node.GetComponent<WorldNode>();
			for(int i = 0; i < MapGenerator.ROWS; i++)
			{
				for(int j = 0; j < MapGenerator.COLS; j++)
				{
					if(wnode.map[i, j] == MapGenerator.FLOOR)
					{
						float mapWidth = MapGenerator.COLS*MeshGenerator.SQUARE_SIZE;
						float mapHeight = MapGenerator.ROWS*MeshGenerator.SQUARE_SIZE;
						float x = node.transform.position.x-mapWidth/2+j*MeshGenerator.SQUARE_SIZE+MeshGenerator.SQUARE_SIZE/2;
						float y = node.transform.position.y+mapHeight/2-i*MeshGenerator.SQUARE_SIZE-MeshGenerator.SQUARE_SIZE/2;
						Debug.DrawLine(new Vector2(x-0.5f, y), new Vector2(x+0.5f, y), Color.cyan, 100f);
					}
				}
			}
		}
	}

	//called by GameState in masterreset
	public void Reset()
	{
		if(nodes.Count == 0)
			generateWorldNodes();
		resetWorldNodes();
		generateMapMeshCollider();
		placeSpawnAndExit();
		generateBuildings();
		generateWildlife();
		foreach(GameObject node in nodes)
			node.GetComponent<WorldNode>().ParentReset();
	}

	//place spawn and exits in each node
	private void placeSpawnAndExit()
	{
		foreach(GameObject node in nodes)
		{
			GameObject spawn = node.transform.Find("PlayerSpawn").gameObject;
			GameObject exit = node.transform.Find("PlayerExit").gameObject;
			spawn.transform.position = GetValidPoint(node);
			exit.transform.position = GetValidPoint(node);
		}
	}

	//populates each worldnode with some wildlife
	private void generateWildlife()
	{
		foreach(GameObject node in nodes)
		{
			WorldNode wnode = node.GetComponent<WorldNode>();
			//get list of valid wildlife spawn points
			List<Vector2> bigPoints = getPoints(node, Random.Range(0, 4));
			List<Vector2> smallPoints = getPoints(node, Random.Range(0, 10));
			useAnimalPool(node, wnode.bigAnimalPool, bigPoints, BIG_ANIMAL);
			useAnimalPool(node, wnode.smallAnimalPool, smallPoints, SMALL_ANIMAL);
		}
	}
	private void useAnimalPool(GameObject node, List<GameObject> pool, List<Vector2> points, string size)
	{
		WorldNode wnode = node.GetComponent<WorldNode>();
		for(int i = 0; i < pool.Count; i++)
		{
			if(points.Count == 0)
			{
				for(int j = i; j < pool.Count; j++)
					pool[j].SetActive(false);
				break;
			}
			Vector2 point = points[0];
			GameObject poolObject = pool[i];
			Wildlife wildlife = poolObject.GetComponent<Wildlife>();
			poolObject.transform.position = wildlife.SetFloorPosition(point);
			poolObject.SetActive(true);
			wildlife.Reset();
			points.Remove(point);
		}
		for(int i = 0; i < points.Count; i++)
		{
			Vector2 point = points[i];
			List<GameObject> animals = beastiary[size];
			GameObject animal = Instantiate(animals[Random.Range(0, animals.Count)], node.transform.Find("Wildlife").Find(size));
			Wildlife wildlife = animal.GetComponent<Wildlife>();
			wildlife.Init();
			wildlife.SetNodeID(nodes.IndexOf(node));
			animal.transform.position = wildlife.SetFloorPosition(point);
			wnode.AddPoolObject(animal, pool);
		}
	}


	//use reservedMap to determine where buildings and future things are placed
	private void generateBuildings()
	{
		activeBuilding.GetComponent<Interior>().Start();//called to initialize interior's mesh pool
		activeBuilding.SetActive(false);
		foreach(GameObject node in nodes)
		{
			WorldNode wnode = node.GetComponent<WorldNode>();
			List<Vector2> points = getPoints(node, Random.Range(0, 5)); 
		
			//use building object pool or spawn new ones
			for(int i = 0; i < wnode.buildingPool.Count; i++)
			{
				if(points.Count == 0)
				{
					for(int j = i; j < wnode.buildingPool.Count; j++)
						wnode.buildingPool[j].SetActive(false);
					break;
				}
				Vector2 point = points[0];
				GameObject poolObject = wnode.buildingPool[i];
				Building building = poolObject.GetComponent<Building>();
				poolObject.transform.position = building.SetFloorPosition(point);
				building.Reset();
				poolObject.SetActive(true);
				points.Remove(point);
			}
			for(int i = 0; i < points.Count; i++)
			{
				Vector2 point = points[i];
				GameObject obj = Instantiate(buildingPrefabs[0], node.transform.Find("Buildings"));
				Building building = obj.GetComponent<Building>();
				building.Init();
				obj.transform.position = building.SetFloorPosition(point);
				wnode.AddPoolObject(obj, wnode.buildingPool);
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

	//deactivates all world nodes
	private void resetWorldNodes()
	{
		foreach(GameObject node in nodes)
			node.SetActive(false);
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

	//returns a point in the node that is still empty floor
	public Vector2 GetValidPoint(GameObject node)
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
				reserveMapPoint(node, row, col);
				return ConvertMapToWorld(row, col, wnode.nodeID);
			}
		}
	}
	
	//returns world coords of closest map row,col pair from pos
  	public Vector2Int NearestMapPair(Vector2 pos, int nodeID)
	{
		int[,] map = nodes[nodeID].GetComponent<WorldNode>().map;
    	float mapWidth = MapGenerator.COLS*MeshGenerator.SQUARE_SIZE;
    	float mapHeight = MapGenerator.ROWS*MeshGenerator.SQUARE_SIZE;
    	Vector2Int closest = new Vector2Int(-1, -1);
    	float min = -1;
    	for(int i = 0; i < MapGenerator.ROWS; i++)
    	{
      		for(int j = 0; j < MapGenerator.COLS; j++)
      		{
        		if(map[i,j] == MapGenerator.FLOOR)
        		{
					Vector2 coords = ConvertMapToWorld(i, j, nodeID);
					float distance = (pos.x-coords.x)*(pos.x-coords.x)+(pos.y-coords.y)*(pos.y-coords.y);
					if(distance < min || min < 0)
					{
						min = distance;
						closest = new Vector2Int(i, j);
					}
				}
      		}
    	}
		
    	return closest;
	}

	public static List<Vector2> GetNeighbors(int row, int col, int nodeID)
	{
		List<Vector2> neighbors = new List<Vector2>();
		int[,] map = World.nodes[nodeID].GetComponent<WorldNode>().map;
		//top neighbor
		if(row > 0)
			if(map[row-1, col] == MapGenerator.FLOOR)
				neighbors.Add(World.ConvertMapToWorld(row, col, nodeID));
		//bottom neighbor
		if(row < MapGenerator.ROWS)
			if(map[row+1, col] == MapGenerator.FLOOR)
				neighbors.Add(World.ConvertMapToWorld(row, col, nodeID));
		//left neighbor
		if(col > 0)
			if(map[row, col-1] == MapGenerator.FLOOR)
				neighbors.Add(World.ConvertMapToWorld(row, col, nodeID));
		//right neighbor
		if(col < MapGenerator.COLS)
			if(map[row, col+1] == MapGenerator.FLOOR)
				neighbors.Add(World.ConvertMapToWorld(row, col, nodeID));
		return neighbors;
	}

	//converts map row,col pair to world coords
	public static Vector2 ConvertMapToWorld(int row, int col, int nodeID)
	{
    	float mapWidth = MapGenerator.COLS*MeshGenerator.SQUARE_SIZE;
    	float mapHeight = MapGenerator.ROWS*MeshGenerator.SQUARE_SIZE;
		float x = NODE_SPACING*nodeID-mapWidth/2+col*MeshGenerator.SQUARE_SIZE+MeshGenerator.SQUARE_SIZE/2;
		float y = mapHeight/2-row*MeshGenerator.SQUARE_SIZE-MeshGenerator.SQUARE_SIZE/2;
		return new Vector2(x, y);
	}

	private void reserveMapPoint(GameObject node, int row, int col)
	{
		WorldNode wnode = node.GetComponent<WorldNode>();
		wnode.map[row, col] = RESERVED;
	}	
	
	//return list of valid points
	private List<Vector2> getPoints(GameObject node, int count)
	{
		List<Vector2> points = new List<Vector2>();
		for(int i = 0; i < count; i++)
		{
			Vector2 point = GetValidPoint(node);
			points.Add(point);	
		}
		return points;
	}
}
