using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: handles creation of world (spawn critters, interactables, etc)

public class World : MonoBehaviour
{
	public static readonly int RESERVED = 2;
	public static readonly int WORLD_SIZE = GameState.DAYS_TO_WIN;
	public static readonly float NODE_SPACING = MapGenerator.COLS*MeshGenerator.SQUARE_SIZE;
	public static List<GameObject> nodes {get; private set;}
	public static List<WorldNode> wnodes {get; private set;}
	public static GameObject activeBuilding {get; private set;}
	public List<GameObject> buildingPrefabs;
	public List<GameObject> villagerPrefabs;
	public List<GameObject> guardPrefabs;
	public List<GameObject> bigAnimalPrefabs;
	public List<GameObject> smallAnimalPrefabs;
	public Dictionary<string, List<GameObject>> beastiary;
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
		wnodes = new List<WorldNode>();
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
						Debug.DrawLine(new Vector2(x-0.5f, y), new Vector2(x+0.5f, y), Color.red, 1f);
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
		activeBuilding.GetComponent<Interior>().Start();//called to initialize interior's mesh pool
		activeBuilding.SetActive(false);
		for(int nodeID = 0; nodeID < WORLD_SIZE; nodeID++)
		{
			generateMapMeshCollider(nodeID);
			placeSpawnAndExit(nodeID);
			generateBuildings(nodeID);
			placeMapItem(nodeID);
			generateWildlife(nodeID);
			wnodes[nodeID].ParentReset();
		}
	}

	//place spawn and exits in each node
	private void placeSpawnAndExit(int nodeID)
	{
		GameObject node = nodes[nodeID];
		GameObject spawn = node.transform.Find("PlayerSpawn").gameObject;
		GameObject exit = node.transform.Find("PlayerExit").gameObject;
		spawn.transform.position = GetValidPoints(nodeID, 1, true)[0];
		exit.transform.position = GetValidPoints(nodeID, 1, true)[0];
	}

	//populates each worldnode with some wildlife
	private void generateWildlife(int nodeID)
	{
		GameObject node = nodes[nodeID];
		WorldNode wnode = wnodes[nodeID];
		//get list of valid wildlife spawn points
		List<Vector2> bigPoints = GetValidPoints(nodeID, Random.Range(0, 4));
		List<Vector2> smallPoints = GetValidPoints(nodeID, Random.Range(0, 10));
		useAnimalPool(nodeID, wnode.bigAnimalPool, bigPoints, BIG_ANIMAL);
		useAnimalPool(nodeID, wnode.smallAnimalPool, smallPoints, SMALL_ANIMAL);
	}

	private void useAnimalPool(int nodeID, List<GameObject> pool, List<Vector2> points, string size)
	{
		GameObject node = nodes[nodeID];
		WorldNode wnode = wnodes[nodeID];
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
			wildlife.SetNodeID(nodeID);
			animal.transform.position = wildlife.SetFloorPosition(point);
			wnode.AddPoolObject(animal, pool);
		}
	}


	//use reservedMap to determine where buildings and future things are placed
	private void generateBuildings(int nodeID)
	{
		GameObject node = nodes[nodeID];
		WorldNode wnode = wnodes[nodeID];
		List<Vector2> points = GetValidPoints(nodeID, Random.Range(3, 6), true); 
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
			if(poolObject == wnode.buildingPool[0])
				building.SetType(Building.Type.BARRACKS);
			else
				building.SetType(Building.Type.HOME);
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
			building.SetNodeID(nodeID);
			if(wnode.buildingPool.Count == 0)
				building.SetType(Building.Type.BARRACKS);
			else
				building.SetType(Building.Type.HOME);
			obj.transform.position = building.SetFloorPosition(point);
        	building.Reset();
			wnode.AddPoolObject(obj, wnode.buildingPool);
		}
	}

	private void placeMapItem(int nodeID)
	{
		Debug.Log("Map has been placed!");
		GameObject objects = wnodes[nodeID].buildingPool[1].GetComponent<Building>().objects;
		objects.transform.Find("Chest").Find("InteractableChest").GetComponent<Container>().SetHasMap(true);
	}
	
	//rolls a new map, mesh, and pool of colliders for walls
	private void generateMapMeshCollider(int nodeID)
	{
		GameObject node = nodes[nodeID];
		WorldNode wnode = wnodes[nodeID];
		int[,] map = mapGen.GenerateMap();
		wnode.SetMap(map);
		Mesh mesh = meshGen.GenerateMesh(map);
		wnode.meshFilter.mesh = mesh;
		collGen.GenerateCollider(node);
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
		wnodes.Add(startNode.GetComponent<WorldNode>());
		for(int nodeID = 1; nodeID < WORLD_SIZE; nodeID++)
		{
			GameObject node = Instantiate(startNode, trans);
			node.name = "Node" + nodeID;
			nodes.Add(node);
			node.GetComponent<Transform>().position = new Vector2(nodeID*NODE_SPACING, 0f);
			WorldNode wnode = node.GetComponent<WorldNode>();
			wnode.SetNodeID(nodeID);
			wnodes.Add(wnode);
		}
	}

	//returns world coords of closest map row,col pair from pos
  	public static int[] NearestMapPair(Vector2 pos, int nodeID)
	{
		int[,] map = wnodes[nodeID].map;
    	int[] closest = new int[2];
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
						closest[0] = i;
						closest[1] = j;
					}
				}
      		}
    	}
    	return closest;
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

	//return list of valid points
	public List<Vector2> GetValidPoints(int nodeID, int count, bool isPermanent=false)
	{
		List<Vector2> points = new List<Vector2>();
		for(int i = 0; i < count; i++)
		{
			int[,] map = wnodes[nodeID].map;	
			while(true)
			{
				int row = Random.Range(1, MapGenerator.COLS-2);
				int col = Random.Range(1, MapGenerator.ROWS-2);
				if(map[row, col] == MapGenerator.FLOOR)
				{
					if(isPermanent)
						reserveMapPoint(nodeID, row, col);
					points.Add(ConvertMapToWorld(row, col, nodeID));
					break;
				}
			}
		}
		return points;
	}

	private void reserveMapPoint(int nodeID, int row, int col)
	{
		int[,] map = wnodes[nodeID].map;
		map[row, col] = RESERVED;
	}	
}
