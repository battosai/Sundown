using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding
{
    //bit order: left|bottom|top|right
    public static readonly int RIGHT = 1;
    public static readonly int TOP = 2;
    public static readonly int TOPRIGHT = 3; 
    public static readonly int BOTTOM = 4;
    public static readonly int BOTTOMRIGHT = 5;
    public static readonly int LEFT = 8;
    public static readonly int TOPLEFT = 10;
    public static readonly int BOTTOMLEFT = 12;
    private static Dictionary<int, int[]> splitDirections = new Dictionary<int, int[]>() 
    {
        //directions are based on row,col orientation
        {TOP, new int[]{-1, 0}},
        {BOTTOM, new int[]{1, 0}},
        {RIGHT, new int[]{0, 1}},
        {LEFT, new int[]{0, -1}},
        {TOPRIGHT, new int[]{-1, 1}},
        {TOPLEFT, new int[]{-1, -1}},
        {BOTTOMRIGHT, new int[]{1, 1}}, 
        {BOTTOMLEFT, new int[]{1, -1}}
    };

	//finds next jump point recursively
    public static Node Jump(Node startNode, Node endNode, int dx, int dy, Node[,] nodeMap, int nodeID)
    {
		//next node in parental direction
		Node nextNode = nodeMap[startNode.row+dy, startNode.col+dx];
		if(nextNode == null)	
			return null;
		if(nextNode.pos == endNode.pos)
			return nextNode;	
		//diagonals
		if(dx != 0 && dy != 0)
		{
			//check for forced neighbors only
			List<Node> neighbors = GetNeighbors(nextNode, nodeMap, nodeID);
			if(neighbors.Count < 8)
			{
				if(nodeMap[startNode.row, startNode.col+dx] == null)
					if(nodeMap[startNode.row, startNode.col+dx+dx] != null)
						return nextNode;						
				if(nodeMap[startNode.row+dy, startNode.col] == null)
					if(nodeMap[startNode.row+dy+dy, startNode.col] != null)
						return nextNode;
			}
			if(Jump(nextNode, endNode, dx, 0, nodeMap, nodeID) != null || Jump(nextNode, endNode, 0, dy, nodeMap, nodeID) != null)
				return nextNode;
		}
		else
		{
			if(dx != 0)
			{
				if(nodeMap[startNode.row+1, startNode.col+dx] == null)
					if(nodeMap[startNode.row+1, startNode.col+dx+dx] != null)
						return nextNode;
				if(nodeMap[startNode.row-1, startNode.col+dx] == null)
					if(nodeMap[startNode.row-1, startNode.col+dx+dx] != null)
						return nextNode;
			}
			else if(dy != 0)
			{
				if(nodeMap[startNode.row+dy, startNode.col+1] == null)
					if(nodeMap[startNode.row+dy+dy, startNode.col+1] != null)
						return nextNode;
				if(nodeMap[startNode.row+dy, startNode.col-1] == null)
					if(nodeMap[startNode.row+dy+dy, startNode.col-1] != null)
						return nextNode;
			}
		}
        return Jump(nextNode, endNode, dx, dy, nodeMap, nodeID);
    }
    
    public static List<Vector2> AStar(Vector2 start, Vector2 destination, Node[,] nodeMap, int nodeID)
	{
		float time = Time.realtimeSinceStartup;
		List<Vector2> path = new List<Vector2>();
		List<Node> visited = new List<Node>();
		Stack<Node> stack = new Stack<Node>(); 
		int[] mapStart = World.NearestMapPair(start, nodeID);
		Node root = nodeMap[mapStart[0], mapStart[1]];
		root.Calculate(null);
		stack.Push(root);
		while(stack.Count > 0)
		{
			Node currNode = stack.Pop();
			if(currNode.pos == destination)
			{
				while(currNode != root)
				{
					path.Add(currNode.pos);
					Debug.DrawLine(new Vector3(currNode.pos.x-5f, currNode.pos.y, 0f), new Vector3(currNode.pos.x+5f, currNode.pos.y, 0f), Color.cyan, 1f);
					currNode = currNode.parent;
				}
				path.Add(root.pos);
				path.Reverse();
				Debug.Log("Path discovered!");
				Debug.Log("[Info] Visited "+visited.Count+" nodes");
				Debug.Log("[Info] Finished in "+(Time.realtimeSinceStartup-time)+"s");
				return path;
			}
			else
			{
				List<Node> neighbors = GetNeighbors(currNode, nodeMap, nodeID);
				foreach(Node neighbor in neighbors)
				{
					if(visited.Contains(neighbor) || stack.Contains(neighbor))
					{
						if(currNode.cost+Vector2.Distance(currNode.pos, neighbor.pos) < neighbor.cost)
							neighbor.Calculate(currNode);
					}
					else
					{
						visited.Add(neighbor);
						neighbor.Calculate(currNode);
						stack.Push(neighbor);
					}
				}
                stack = new Stack<Node>(bubbleSort(stack.ToArray()));
			}
		}
		Debug.Log("[Warning] Could not find path to destination");
		return null;
	}

    //checks specific neighbor
    public static bool CheckNeighbor(Node origin, Node[,] nodeMap, int nodeID, int direction)
    {
        int row = origin.row;
        int col = origin.col;
        int[,] map = World.wnodes[nodeID].map;
        int[] directions = splitDirections[direction];
        return map[row+directions[0], row+directions[1]] == MapGenerator.FLOOR;
    }

	//return list of neighbors as pathnodes
	public static List<Node> GetNeighbors(Node origin, Node[,] nodeMap, int nodeID)
	{
		int row = origin.row;
		int col = origin.col;
		List<Node> neighbors = new List<Node>();
		int[,] map = World.wnodes[nodeID].map;
		//top neighbor
		if(row > 0)
			if(map[row-1, col] == MapGenerator.FLOOR)
				neighbors.Add(nodeMap[row-1, col]);
		//bottom neighbor
		if(row < MapGenerator.ROWS)
			if(map[row+1, col] == MapGenerator.FLOOR)
				neighbors.Add(nodeMap[row+1, col]);
		//left neighbor
		if(col > 0)
			if(map[row, col-1] == MapGenerator.FLOOR)
				neighbors.Add(nodeMap[row, col-1]);
		//right neighbor
		if(col < MapGenerator.COLS)
			if(map[row, col+1] == MapGenerator.FLOOR)
				neighbors.Add(nodeMap[row, col+1]);
        //top left neighbor
        if(row > 0 && col > 0)
            if(map[row-1, col-1] == MapGenerator.FLOOR)
                neighbors.Add(nodeMap[row-1, col-1]);
        //top right neighbor
        if(row > 0 && col < MapGenerator.COLS)
            if(map[row-1, col+1] == MapGenerator.FLOOR)
                neighbors.Add(nodeMap[row-1, col+1]);
        //bottom left neighbor
        if(row < MapGenerator.ROWS && col > 0)
            if(map[row+1, col-1] == MapGenerator.FLOOR)
                neighbors.Add(nodeMap[row+1, col-1]);
        //bottom right neighbor
        if(row < MapGenerator.ROWS && col < MapGenerator.COLS)
            if(map[row+1, col+1] == MapGenerator.FLOOR)
                neighbors.Add(nodeMap[row+1, col+1]);
		return neighbors;
	}

	public class Node
	{
		public float estimate, cost, total;
		public int weight, row, col;
		public Vector2 pos;
		public Node parent;
		public Node(int row, int col, int nodeID, Node parent)
		{
			this.row = row;
			this.col = col;
			this.pos = World.ConvertMapToWorld(row, col, nodeID);
			this.parent = parent;
		}
		public void Guesstimate(Vector2 destination)
		{
			//this heuristic has to be admissible i.e. cannot overestimate the real cost
			this.estimate = Vector2.Distance(this.pos, destination);
		}
		public void Calculate(Node parent)
		{
			if(parent != null)
				this.cost = parent.cost+Vector2.Distance(this.pos, parent.pos);
			else
				this.cost = 0f;
			this.total = this.cost+this.estimate;
			this.parent = parent;
		}
		public static Node[,] MakeNodeMap(Vector2 destination, int[,] map, int nodeID)
		{
			Node[,] nodeMap = new Node[MapGenerator.ROWS, MapGenerator.COLS];
			for(int i = 0; i < MapGenerator.ROWS; i++)
			{
				for(int j = 0; j < MapGenerator.COLS; j++)
				{
					if(map[i, j] == MapGenerator.FLOOR)
					{
						Node node = new Node(i, j, nodeID, null);
						node.Guesstimate(destination);
						nodeMap[i, j] = node;
					}
				}
			}
			return nodeMap;
		}
	}

    //sorts an array of Nodes
    private static Node[] bubbleSort(Node[] unsorted)
    {
        for(int c = 0; c < unsorted.Length-1; c++)
        {
            for(int i = 1; i < unsorted.Length; i++)
            {
                if(unsorted[i-1].total < unsorted[i].total)
                {
                    Node swapper = unsorted[i-1];
                    unsorted[i-1] = unsorted[i];
                    unsorted[i] = swapper;
                }
            }
        }
        //caller is responsible for converting array to proper ienumerable
        return unsorted;
    }
}