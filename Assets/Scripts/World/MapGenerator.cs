using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: generates map procedurally that world references

public class MapGenerator : MonoBehaviour
{
  public static readonly int COLS = 50;
  public static readonly int ROWS = 50;
  public static readonly int EQUAL_NEIGHBORS = 4;
  private readonly int SQUARE_SIZE = 2;
  private readonly int SMOOTH_EPOCHS = 5;

  public static int[,] map {get; private set;}

  private string seed;

  [Range(0, 100)]
  public int randomFillPercent;
  public bool isRandomSeed;

	// Use this for initialization
	void Start()
	{
    generateMap();
	}

	// Update is called once per frame
	void Update()
	{
    if (Input.GetMouseButtonDown(0))
    {
      generateMap();
    }
	}

  private void generateMap()
  {
    seed = "";
    map = new int[ROWS, COLS];
    randomFillMap();
    for(int i = 0; i < SMOOTH_EPOCHS; i++)
    { 
      smoothMap();
    }
    MeshGenerator meshGen = GetComponent<MeshGenerator>();
    meshGen.GenerateMesh(map, SQUARE_SIZE);
  }

  private List<Coord> getRegion(int startRow, int startCol)
  {
    List<Coord> tiles = new List<Coord>();
    bool[,] isVisited = new bool[ROWS, COLS];
    int tileType = map[startRow, startCol];
    Queue<Coord> queue = new Queue<Coord>();
    queue.Enqueue(new Coord(startRow, startCol));
    isVisited[startRow, startCol] = true;
    while(queue.Count > 0)
    {
      Coord tile = queue.Dequeue();
      tiles.Add(tile);
      for(int i = tile.row-1; i <= tile.row+1; i++)
      {
        for(int j = tile.col-1; j <= tile.col+1; j++)
        {
          if(i < 0 || i >= ROWS || j < 0 || j >= COLS)
            continue;
          if(i == tile.row && j == tile.col)
            continue;
          if(!isVisited[i, j] && map[i, j] == tileType)
          {
            queue.Enqueue(new Coord(i, j));
            isVisited[i, j] = true;
          }
        }
      }
    }
    return tiles;
  }

  struct Coord
  {
    public int row;
    public int col;
    public Coord(int a, int b)
    {
      row = a;
      col = b;
    }
  }

  //randomly fills map based on randomfillpercent
  private void randomFillMap()
  {
    //if not random, seed will be empty thus returning same one each time
    if(isRandomSeed)
      seed = Time.time.ToString();
    System.Random rand = new System.Random(seed.GetHashCode());
    for(int i = 0; i < ROWS; i++)
    {
      for(int j = 0; j < COLS; j++)
      {
        if(i == 0 || j == 0 || i == ROWS-1 || j == COLS-1)
        {
          map[i, j] = 1;
        }
        else
        {
          map[i, j] = (rand.Next(0, 100) < randomFillPercent)? 1 : 0;
        }
      }
    }
  }

  //smooths randomly filled map
  private void smoothMap()
  {
    for(int i = 0; i < ROWS; i++)
    {
      for(int j = 0; j < COLS; j++)
      {
        if(i == 0 || j == 0 || i == ROWS-1 || j == COLS-1)
          continue;
        int neighbors = countWallNeighbors(i, j);
        if(neighbors > EQUAL_NEIGHBORS)
          map[i, j] = 1;
        else if(neighbors < EQUAL_NEIGHBORS)
          map[i, j] = 0;
      }
    }
  }

  //returns number of neighbors that are walls
  private int countWallNeighbors(int x, int y)
  {
    int count = 0;
    for(int i = x-1; i <= x+1; i++)
    {
      for(int j = y-1; j <= y+1; j++)
      {
        if(i < 0 || i >= ROWS || j < 0 || j >= COLS)
          continue;
        if(i == x && j == y)
          continue;
        count += map[i, j];
      }
    }
    return count;
  }

  // void OnDrawGizmos()
  // {
  //   if(map != null)
  //   {
  //     for(int x = 0; x < ROWS; x++)
  //     {
  //       for(int y = 0; y < COLS; y++)
  //       {
  //         Gizmos.color = (map[x, y] == 1)? Color.black : Color.white;
  //         Vector3 pos = new Vector3(-ROWS/2 + x + .5f, -COLS/2 + y + .5f, 0);
  //         Gizmos.DrawCube(pos, Vector3.one);
  //       }
  //     }
  //   }
  // }
}
