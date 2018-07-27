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

	protected List<Vector2> astarPath(Vector2 destination)
	{
		List<Vector2> path = new List<Vector2>();
		// List<pathNode> visited = new List<pathNode>();
		Dictionary<Vector2Int, pathNode> visited = new Dictionary<Vector2Int, pathNode>();
		Stack stack = new Stack(); 
		int[] mapStart = World.NearestMapPair(trans.position, nodeID);
		pathNode root = new pathNode(mapStart[0], mapStart[1], nodeID, null);
		path.Add(root.pos);
		stack.Push(root);
		while(stack.Count > 0)
		{
			pathNode currNode = (pathNode)stack.Pop();
			if(currNode.pos == destination)
			{
				break;
			}
			else
			{
				visited[new Vector2Int(currNode.row, currNode.col)] = currNode;
				List<pathNode> neighbors = getNeighbors(currNode);
				foreach(pathNode neighbor in neighbors)
				{
					//PROBLEM: these are not unique pathNodes for an individual entry of a map pair
					//every call to getNeighbors creates a new pathNode object and won't be contained in visited technically
					//can't use Vector2Int because we need to update the heuristic estimate if it's already been visited
					//try using dictionary for visited (vector2int, pathNode)
				}
			}
		}
		return path;
	}

	//return list of neighbors as pathnodes
	protected List<pathNode> getNeighbors(pathNode origin)
	{
		int row = origin.row;
		int col = origin.col;
		List<pathNode> neighbors = new List<pathNode>();
		int[,] map = World.nodes[nodeID].GetComponent<WorldNode>().map;
		//top neighbor
		if(row > 0)
			if(map[row-1, col] == MapGenerator.FLOOR)
				neighbors.Add(new pathNode(row, col, nodeID, origin));
		//bottom neighbor
		if(row < MapGenerator.ROWS)
			if(map[row+1, col] == MapGenerator.FLOOR)
				neighbors.Add(new pathNode(row, col, nodeID, origin));
		//left neighbor
		if(col > 0)
			if(map[row, col-1] == MapGenerator.FLOOR)
				neighbors.Add(new pathNode(row, col, nodeID, origin));
		//right neighbor
		if(col < MapGenerator.COLS)
			if(map[row, col+1] == MapGenerator.FLOOR)
				neighbors.Add(new pathNode(row, col, nodeID, origin));
		return neighbors;
	}

	protected class pathNode
	{
		public float estimate;
		public int row, col;
		public Vector2 pos;
		public pathNode parent;
		public pathNode(int row, int col, int nodeID, pathNode parent)
		{
			this.row = row;
			this.col = col;
			this.pos = World.ConvertMapToWorld(row, col, nodeID);
			this.parent = parent;
		}
		public void calculate(Vector2 destination)
		{ 
			this.estimate = Mathf.Pow(this.pos.x-destination.x, 2)+Mathf.Pow(this.pos.y-destination.y, 2);
		}
	}
}
