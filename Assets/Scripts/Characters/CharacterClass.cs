using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType {PLAYER, HERO, WILDLIFE, TOWNSPERSON};
public class CharacterClass : MonoBehaviour
{
	public readonly float BASE_SPEED = 20f;
	public CharacterType type {get; private set;}
	public int nodeID {get; private set;}
	public int health {get; private set;}
	public float speed {get; private set;}
	public float floorHeight {get; private set;}
	public Vector2 floorPosition {get; private set;}
	public bool isLeft {get; private set;}
	public GameState gameState {get; private set;}
	public World world {get; private set;}
	public Transform trans {get; private set;}
	public Rigidbody2D rb {get; private set;}
	public SpriteRenderer rend {get; private set;}
	protected PathFinding.Node[,] nodeMap;
	public void SetType(CharacterType type){this.type = type;}
	public void SetNodeID(int id){nodeID = id;}
	public void SetIsLeft(bool isLeft){this.isLeft = isLeft;}
	public void SetHealth(int health){this.health = health;}
	public void SetSpeed(int speed){this.speed = speed;}

	public virtual void Awake()
	{
		gameState = GameObject.Find("GameState").GetComponent<GameState>();
		world = GameObject.Find("World").GetComponent<World>(); 
		trans = GetComponent<Transform>();
		rb = GetComponent<Rigidbody2D>();
		rend = GetComponent<SpriteRenderer>();
	}

	public virtual void Reset()
	{
		speed = BASE_SPEED;
	}

	public virtual void UpdateAnimator()
	{
		Debug.Log(gameObject.name+" has no proper UpdateAnimator() method");
	}
	
	//sets position so that floor position is at target
	public Vector2 SetFloorPosition(Vector2 target)
	{
		float yOffset = rend.bounds.size.y/2;
		return new Vector2(target.x, target.y+yOffset);
	}

	//sets floor height (bottom of sprite), used for collisions and rendering orders
	protected void setFloorHeight()
	{
		floorHeight = trans.position.y-(rend.bounds.size.y/2);
		trans.position = new Vector3(trans.position.x, trans.position.y, floorHeight);
		floorPosition = new Vector2(trans.position.x, floorHeight);
	}

	// protected List<Vector2> astarPath(Vector2 destination)
	// {
	// 	List<Vector2> path = new List<Vector2>();
	// 	List<pathNode> visited = new List<pathNode>();
	// 	Stack<pathNode> stack = new Stack<pathNode>(); 
	// 	int[] mapStart = World.NearestMapPair(trans.position, nodeID);
	// 	pathNode root = nodeMap[mapStart[0], mapStart[1]];
	// 	root.cost = 0f;
	// 	stack.Push(root);
	// 	while(stack.Count > 0)
	// 	{
	// 		pathNode currNode = stack.Pop();
	// 		if(currNode.pos == destination)
	// 		{
	// 			Debug.Log("Path discovered!");
	// 			while(currNode != root)
	// 			{
	// 				path.Add(currNode.pos);
	// 				Debug.DrawLine(new Vector3(currNode.pos.x-5f, currNode.pos.y, 0f), new Vector3(currNode.pos.x+5f, currNode.pos.y, 0f), Color.cyan, 1f);
	// 				currNode = currNode.parent;
	// 			}
	// 			path.Add(root.pos);
	// 			path.Reverse();
	// 			return path;
	// 		}
	// 		else
	// 		{
	// 			List<pathNode> neighbors = getNeighbors(currNode, nodeMap);
	// 			foreach(pathNode neighbor in neighbors)
	// 			{
	// 				if(visited.Contains(neighbor) || stack.Contains(neighbor))
	// 				{
	// 					if(currNode.cost+1 < neighbor.cost)
	// 						neighbor.calculate(currNode);
	// 				}
	// 				else
	// 				{
	// 					visited.Add(neighbor);
	// 					neighbor.calculate(currNode);
	// 					stack.Push(neighbor);
	// 				}
	// 			}
	// 			pathNode[] stackArray = stack.ToArray();
	// 			for(int c = 0; c < stack.Count-1; c++)
	// 			{
	// 				for(int i = 1; i < stack.Count; i++)
	// 				{
	// 					if(stackArray[i-1].total < stackArray[i].total)
	// 					{
	// 						pathNode swapper = stackArray[i-1];
	// 						stackArray[i-1] = stackArray[i];
	// 						stackArray[i] = swapper;
	// 					}
	// 				}
	// 			}
	// 			stack = new Stack<pathNode>(stackArray);
	// 		}
	// 	}
	// 	Debug.Log("[Warning] Could not find path to player");
	// 	return null;
	// }

	// //return list of neighbors as pathnodes
	// protected List<pathNode> getNeighbors(pathNode origin, pathNode[,] nodeMap)
	// {
	// 	int row = origin.row;
	// 	int col = origin.col;
	// 	List<pathNode> neighbors = new List<pathNode>();
	// 	int[,] map = World.nodes[nodeID].GetComponent<WorldNode>().map;
	// 	//top neighbor
	// 	if(row > 0)
	// 		if(map[row-1, col] == MapGenerator.FLOOR)
	// 			neighbors.Add(nodeMap[row-1, col]);
	// 	//bottom neighbor
	// 	if(row < MapGenerator.ROWS)
	// 		if(map[row+1, col] == MapGenerator.FLOOR)
	// 			neighbors.Add(nodeMap[row+1, col]);
	// 	//left neighbor
	// 	if(col > 0)
	// 		if(map[row, col-1] == MapGenerator.FLOOR)
	// 			neighbors.Add(nodeMap[row, col-1]);
	// 	//right neighbor
	// 	if(col < MapGenerator.COLS)
	// 		if(map[row, col+1] == MapGenerator.FLOOR)
	// 			neighbors.Add(nodeMap[row, col+1]);
	// 	return neighbors;
	// }

	// protected class pathNode
	// {
	// 	public float estimate, cost, total;
	// 	public int row, col;
	// 	public Vector2 pos;
	// 	public pathNode parent;
	// 	public pathNode(int row, int col, int nodeID, pathNode parent)
	// 	{
	// 		this.row = row;
	// 		this.col = col;
	// 		this.pos = World.ConvertMapToWorld(row, col, nodeID);
	// 		this.parent = parent;
	// 	}
	// 	public void guesstimate(Vector2 destination)
	// 	{
	// 		this.estimate = Mathf.Pow(this.pos.x-destination.x, 2)+Mathf.Pow(this.pos.y-destination.y, 2);
	// 	}
	// 	public void calculate(pathNode parent)
	// 	{
	// 		this.cost = parent.cost+1;
	// 		this.total = this.cost+this.estimate;
	// 		this.parent = parent;
	// 	}
	// 	public static pathNode[,] makeNodeMap(Vector2 destination, int[,] map, int nodeID)
	// 	{
	// 		pathNode[,] nodeMap = new pathNode[MapGenerator.ROWS, MapGenerator.COLS];
	// 		for(int i = 0; i < MapGenerator.ROWS; i++)
	// 		{
	// 			for(int j = 0; j < MapGenerator.COLS; j++)
	// 			{
	// 				pathNode node = new pathNode(i, j, nodeID, null);
	// 				node.guesstimate(destination);
	// 				nodeMap[i, j] = node;
	// 			}
	// 		}
	// 		return nodeMap;
	// 	}
	// }
}
