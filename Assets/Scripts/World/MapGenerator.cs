using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: generates map procedurally that world references
//NOTE: ALL MULTIDIMENSIONAL STRUCTURES USE (row, col) instead of cartesian

public class MapGenerator : MonoBehaviour
{
  [Range(0, 100)]
  public int randomFillPercent;
  public bool isRandomSeed;
  private readonly int COLS = 50;
  private readonly int ROWS = 50;
  private readonly int EQUAL_NEIGHBORS = 4;
  private readonly int SMOOTH_EPOCHS = 5;
  private readonly int WALL = 1;
  private readonly int FLOOR = 0;
  private readonly int WALL_FILTER = 10;
  private readonly int FLOOR_FILTER = 10;
  private string seed = "";

  public int[,] generateMap()
  {
    int[,] map = new int[ROWS, COLS];
    randomFillMap(map);
    for(int i = 0; i < SMOOTH_EPOCHS; i++)
    { 
      smoothMap(map);
    }
    filterMapRegions(map);
    return map;
  }

  //removes regions that are too small
  private void filterMapRegions(int[,] map)
  {
    List<List<Coord>> walls = getRegionsOfType(WALL);
    foreach(List<Coord> region in walls)
    {
      if(region.Count < WALL_FILTER)
      {
        foreach(Coord tile in region)
        {
          map[tile.row, tile.col] = FLOOR;
        }
      }
    }
    List<List<Coord>> floors = getRegionsOfType(FLOOR);
    foreach(List<Coord> region in floors)
    {
      if(region.Count < FLOOR_FILTER)
      {
        foreach(Coord tile in region)
        {
          map[tile.row, tile.col] = WALL;
        }
      }
    }
  }

  //returns a list of all regions (as lists of tiles) in the map of tileType
  private List<List<Coord>> getRegionsOfType(int tileType)
  {
    List<List<Coord>> regions = new List<List<Coord>>();
    bool[,] isVisited = new bool[ROWS, COLS];
    for(int i = 0; i < ROWS; i++)
    {
      for(int j = 0; j < COLS; j++)
      {
        if(!isVisited[i, j] && map[i, j] == tileType)
        {
          List<Coord> region = getRegion(i, j);
          foreach(Coord tile in region)
            isVisited[tile.row, tile.col] = true;
          regions.Add(region);
        }
      }
    }
    return regions;
  }

  //returns a list of tiles for one region of the starting tile type
  //implemented as a floodfill queue algorithm
  private List<Coord> getRegion(int startRow, int startCol)
  {
    List<Coord> tiles = new List<Coord>();
    Queue<Coord> queue = new Queue<Coord>();
    bool[,] isVisited = new bool[ROWS, COLS];
    int tileType = map[startRow, startCol];
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

  //used instead of Vector2 because Vector2 uses x and y which could be confusing
  private struct Coord
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
  private void randomFillMap(int[,] map)
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
          map[i, j] = (rand.Next(0, 100) < randomFillPercent)? WALL : FLOOR;
        }
      }
    }
  }

  //smooths randomly filled map
  private void smoothMap(int[,] map)
  {
    for(int i = 0; i < ROWS; i++)
    {
      for(int j = 0; j < COLS; j++)
      {
        if(i == 0 || j == 0 || i == ROWS-1 || j == COLS-1)
          continue;
        int neighbors = countWallNeighbors(i, j);
        if(neighbors > EQUAL_NEIGHBORS)
          map[i, j] = WALL;
        else if(neighbors < EQUAL_NEIGHBORS)
          map[i, j] = FLOOR;
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
