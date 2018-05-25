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
  private readonly int ALL = 8;
  private readonly int HALF = 4;
  private readonly int SMOOTH_EPOCHS = 5;
  private readonly int WALL = 1;
  private readonly int FLOOR = 0;
  private readonly int WALL_FILTER = 10;
  private readonly int FLOOR_FILTER = 10;
  private readonly int CORRIDOR_HEIGHT = 3;
  private readonly int CORRIDOR_WIDTH = 3;
  private int randomizer = 0;
  private string seed = "";

  public int[,] GenerateMap()
  {
    int[,] map = new int[ROWS, COLS];
    randomFillMap(map);
    for(int i = 0; i < SMOOTH_EPOCHS; i++)
    { 
      smoothMap(map);
    }
    //maybe make this a List<List<Room>> so that it returns a list for each room type!!!
    //will help when generating the polygon collider
    List<Room> rooms = filterMapRegions(map);
    connectClosestRooms(map, rooms);
    return map;
  }

  private class Room
  {
    public int size;
    public List<Coord> tiles;
    public List<Coord> edgeTiles;
    public List<Room> connected;

    public Room(int[,] map, List<Coord> tiles)
    {
      this.tiles = tiles;
      size = tiles.Count;
      connected = new List<Room>();
      edgeTiles = new List<Coord>();
      foreach(Coord tile in tiles)
        if(countWallNeighbors(map, tile.row, tile.col, HALF) > 0)
          edgeTiles.Add(tile);
    }

    public static void connectRooms(Room a, Room b)
    {
      a.connected.Add(b);
      b.connected.Add(a);
    }
  }

  //removes regions that are too small, returns list of floor regions as rooms
  private List<Room> filterMapRegions(int[,] map)
  {
    List<List<Coord>> walls = getRegionsOfType(map, WALL);
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
    List<Room> rooms = new List<Room>();
    List<List<Coord>> floors = getRegionsOfType(map, FLOOR);
    foreach(List<Coord> region in floors)
    {
      if(region.Count < FLOOR_FILTER)
      {
        foreach(Coord tile in region)
        {
          map[tile.row, tile.col] = WALL;
        }
      }
      else
        rooms.Add(new Room(map, region));
    }
    return rooms;
  }

  //creates corridors between each room's closest neighbor
  //MAYBE JUST MAKE IT FIND ONE ROOMS CLOSEST NEIGHBOR, THEN THAT GROUPS CLOSEST, ETC. so that all are connected
  private void connectClosestRooms(int[,] map, List<Room> rooms)
  {
    foreach(Room a in rooms)
    {
      int minDistance = 0;
      Coord minTileA, minTileB;
      Room minRoomB;
      foreach(Room b in rooms)
      {
        if(a == b)
          continue;
        if(a.connected.Contains(b))
        {
          minDistance = 0;
          break;
        }
        foreach(Coord tileA in a.edgeTiles)
        {
          foreach(Coord tileB in b.edgeTiles)
          {
            int distance = (int) Mathf.Pow(tileA.row-tileB.row, 2) + Mathf.Pow(tileA.col-tileB.col, 2);
            if(distance < minDistance || minDistance == 0)
            {
              minDistance = distance;
              minTileA = tileA;
              minTileB = tileB;
              minRoomB = b;
            }
          }
        }
      }
      if(minDistance > 0)
        createCorridor(map, a, minRoomB, minTileA, minTileB);
    }
  }

  // //unused, just making for testing later
  // //connects all rooms after theyve connected with closest neighbors
  // private void connectRoomGroups(int[,] map, List<Room> rooms)
  // {
  //   //choose one room group as already linked
  //   List<Room> linked = new List<Room>();
  //   linked.Add(rooms[0]);
  //   foreach(Room room in rooms[0].connected)
  //     linked.Add(room);
  //   //HARD TO DEFINE LINKED ROOMS BECAUSE WOULD HAVE TO GO DOWN FAR
  // }

  // private List<Room> getLinkedRooms(Room origin)
  // {
  //   List<Room> linked = new List<Room>();
  //   linked.Add(origin);
  //   Queue<Room> unchecked = new Queue<Room>();
  //   //DOESNT LIKE THIS ENQUEE STATEMENT
  //   //unchecked.Enqueue(origin);
  //   while(unchecked.Count > 0)
  //   {
  //     if(linked.Contains(unchecked[0]))
  //       unchecked.Dequeue();
  //     else
  //       break;
  //   }
  //   return linked;
  // }

  private void createCorridor(int[,] map, Room roomA, Room roomB, Coord tileA, Coord tileB)
  {
    Room.connectRooms(roomA, roomB);
    //temp so corridors can be visualized
    Vector3 start = new Vector3(-COLS/2 + 0.5f + roomA.col, ROWS/2 - 0.5f - roomA.row);
    Vector3 end = new Vector3(-COLS/2 + 0.5f + roomB.col, ROWS/2 - 0.5f - roomB.row);
    Debug.DrawLine(start, end, Color.green, 100);
  }

  //returns a list of all regions (as lists of tiles) in the map of tileType
  private List<List<Coord>> getRegionsOfType(int[,] map, int tileType)
  {
    List<List<Coord>> regions = new List<List<Coord>>();
    bool[,] isVisited = new bool[ROWS, COLS];
    for(int i = 0; i < ROWS; i++)
    {
      for(int j = 0; j < COLS; j++)
      {
        if(!isVisited[i, j] && map[i, j] == tileType)
        {
          List<Coord> region = getRegion(map, i, j);
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
  private List<Coord> getRegion(int[,] map, int startRow, int startCol)
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
    {
      seed = (Time.time + randomizer).ToString();
      randomizer++;
    }
    System.Random rand = new System.Random(seed.GetHashCode());
    for(int i = 0; i < ROWS; i++)
    {
      for(int j = 0; j < COLS; j++)
      {
        if(i == 0 || j == 0 || i == ROWS-1 || j == COLS-1)
          map[i, j] = 1;
        else
          map[i, j] = (rand.Next(0, 100) < randomFillPercent)? WALL : FLOOR;
      }
    }
  }

  //smooths randomly filled map to reduce noise
  private void smoothMap(int[,] map)
  {
    for(int i = 0; i < ROWS; i++)
    {
      for(int j = 0; j < COLS; j++)
      {
        if(i == 0 || j == 0 || i == ROWS-1 || j == COLS-1)
          continue;
        int neighbors = countWallNeighbors(map, i, j, ALL);
        if(neighbors > EQUAL_NEIGHBORS)
          map[i, j] = WALL;
        else if(neighbors < EQUAL_NEIGHBORS)
          map[i, j] = FLOOR;
      }
    }
  }

  //returns number of neighbors (all/half) that are walls
  private int countWallNeighbors(int[,] map, int tileRow, int tileCol, int directions)
  {
    int count = 0;
    if(directions == ALL)
      for(int i = tileRow-1; i <= tileRow+1; i++)
      {
        for(int j = tileCol-1; j <= tileCol+1; j++)
        {
          if(i < 0 || i >= ROWS || j < 0 || j >= COLS)
            continue;
          if(i == tileRow && j == tileCol)
            continue;
          count += map[i, j];
        }
      }
    else if(directions == HALF)
    {
      if(i > 0)
        count += map[i-1, j];
      if(i < ROWS)
        count += map[i+1, j];
      if(j > 0)
        count += map[i, j-1];
      if(j < COLS)
        count += map[i, j+1];
    }
    else
      Debug.Log("[Input Error] Invalid Direction Check");
    return count;
  }
}
