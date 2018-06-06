using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: generates map procedurally that world references
//NOTICE: ALL MULTIDIMENSIONAL STRUCTURES USE (row, col) instead of cartesian

public class MapGenerator : MonoBehaviour
{
  [Range(0, 100)]
  public int randomFillPercent;
  public bool isRandomSeed;
  public List<List<Vector2>> edgePoints;
  private static readonly int COLS = 100;
  private static readonly int ROWS = 100;
  private static readonly int EQUAL_NEIGHBORS = 4;
  private static readonly int SMOOTH_EPOCHS = 5;
  private static readonly int WALL = 1;
  private static readonly int FLOOR = 0;
  private static readonly int WALL_FILTER = 10;
  private static readonly int FLOOR_FILTER = 10;
  private static readonly int CORRIDOR_HEIGHT = 3;
  private static readonly int CORRIDOR_WIDTH = 3;
  private float randomizer = 0f;
  private string seed = "";

  public int[,] GenerateMap()
  {
    int[,] map = new int[ROWS, COLS];
    randomFillMap(map);
    for(int i = 0; i < SMOOTH_EPOCHS; i++)
    {
      smoothMap(map);
    }
    List<Room> rooms = filterMapRegions(map);
    //NOTE: postponing corridor connections
    // connectClosestRooms(map, rooms);
    // connectRoomGroups(map, rooms);
    return map;
  }

  private class Room
  {
    public int size;
    public List<Coord> tiles;
    public List<Coord> edgeTiles;
    public List<Room> connected;

    public Room(){}
    public Room(int[,] map, List<Coord> tiles)
    {
      this.tiles = tiles;
      size = tiles.Count;
      connected = new List<Room>();
      edgeTiles = new List<Coord>();
      foreach(Coord tile in tiles)
      {
        //if any 4D wall neighbors, add to edgeTiles
        if(map[tile.row-1, tile.col] == MapGenerator.WALL)
          edgeTiles.Add(tile);
        else if(map[tile.row+1, tile.col] == MapGenerator.WALL)
          edgeTiles.Add(tile);
        else if(map[tile.row, tile.col-1] == MapGenerator.WALL)
          edgeTiles.Add(tile);
        else if(map[tile.row, tile.col+1] == MapGenerator.WALL)
          edgeTiles.Add(tile);
      }
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
    populateEdgePoints(rooms);
    return rooms;
  }

  //fills up the public attribute of edgepoints
  //edgepoints contains lists of vector2 for each room's edgetiles (2D list)
  private void populateEdgePoints(List<Room> rooms)
  {
    edgePoints = new List<List<Vector2>>();
    foreach(Room room in rooms)
    {
      List<Vector2> edgeGroup = new List<Vector2>();
      foreach(Coord edgeTile in room.edgeTiles)
      {
        Vector2 point = new Vector2(-COLS/2 + 0.5f + edgeTile.col, ROWS/2 - 0.5f - edgeTile.row);
        edgeGroup.Add(point);
      }
      edgePoints.Add(edgeGroup);
      Debug.Log("Room has " + edgeGroup.Count);
    }
  }

  //creates corridors between each room's closest neighbor
  //MAYBE JUST MAKE IT FIND ONE ROOMS CLOSEST NEIGHBOR, THEN THAT GROUPS CLOSEST, ETC. so that all are connected
  private void connectClosestRooms(int[,] map, List<Room> rooms)
  {
    foreach(Room a in rooms)
    {
      int minDistance = 0;
      Coord minTileA = new Coord();
      Coord minTileB = new Coord();
      Room minRoomB = new Room();
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
            int distance = (int)(Mathf.Pow(tileA.row-tileB.row, 2) + Mathf.Pow(tileA.col-tileB.col, 2));
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

  //unused, just making for testing later
  //connects all rooms after theyve connected with closest neighbors
  private void connectRoomGroups(int[,] map, List<Room> rooms)
  {
    List<Room> linked = new List<Room>();
    linked = getLinkedRooms(rooms[0]);
    foreach(Room room in rooms)
    {
      int minDistance = 0;
      Room minLinkedRoom = new Room();
      Room minUnlinkedRoom = new Room();
      Coord minLinkedTile = new Coord();
      Coord minUnlinkedTile = new Coord();
      if(linked.Contains(room))
        continue;
      List<Room> unlinked = getLinkedRooms(room);
      foreach(Room linkedRoom in linked)
        foreach(Room unlinkedRoom in unlinked)
          foreach(Coord linkedTile in linkedRoom.edgeTiles)
            foreach(Coord unlinkedTile in unlinkedRoom.edgeTiles)
            {
              int distance = (int)(Mathf.Pow(linkedTile.row-unlinkedTile.row, 2)+Mathf.Pow(linkedTile.col-unlinkedTile.col, 2));
              if(distance < minDistance || minDistance == 0)
              {
                minDistance = distance;
                minLinkedRoom = linkedRoom;
                minUnlinkedRoom = unlinkedRoom;
                minLinkedTile = linkedTile;
                minUnlinkedTile = unlinkedTile;
              }
            }
      if(minDistance > 0)
        createCorridor(map, minLinkedRoom, minUnlinkedRoom, minLinkedTile, minUnlinkedTile);
      foreach(Room unlinkedRoom in unlinked)
        linked.Add(unlinkedRoom);
    }
    Debug.Log("Total Rooms Linked: " + linked.Count);
  }

  //returns a list of rooms that are linked to each other (connected =/= linked)
  //connected rooms are directly connected, linked can also be indirectly connected
  private List<Room> getLinkedRooms(Room origin)
  {
    List<Room> linked = new List<Room>();
    Queue<Room> queue = new Queue<Room>();
    queue.Enqueue(origin);
    while(queue.Count > 0)
    {
      Room room = queue.Dequeue();
      if(!linked.Contains(room))
        linked.Add(room);
      foreach(Room connection in room.connected)
        if(!queue.Contains(connection) && !linked.Contains(connection))
          queue.Enqueue(connection);
    }
    return linked;
  }

  private void createCorridor(int[,] map, Room roomA, Room roomB, Coord tileA, Coord tileB)
  {
    Room.connectRooms(roomA, roomB);
    //temp so corridors can be visualized
    Vector3 start = new Vector3(-COLS/2 + 0.5f + tileA.col, ROWS/2 - 0.5f - tileA.row);
    Vector3 end = new Vector3(-COLS/2 + 0.5f + tileB.col, ROWS/2 - 0.5f - tileB.row);
    int dx = tileB.col - tileA.col;
    int dy = tileB.row - tileA.row;
    int stepX = Math.Sign(dx);
    int stepY = Math.Sign(dy);
    bool isLeadingX = Mathf.Abs(dx) > Mathf.Abs(dy);
    if(isLeadingX)
    {
      Debug.Log("leading X: slope is " + dy + "/" + dx);
      Debug.DrawLine(start, end, Color.green, 5);
      int i = 0;
      for(int j = 0; Mathf.Abs(j) <= Mathf.Abs(dx); j += stepX)
      {
        if(stepY >= 0)
        {
          if((dy/dx)*j > (i+0.5f))
            i += stepY;
        }
        else
        {
          if((-dy/dx)*j > (i-0.5f))
            i += stepY;
        }
        map[tileA.row+i, tileA.col+j] = FLOOR;
      }
    }
    else
    {
      Debug.Log("leading Y: slope is " + dy + "/" + dx);
      Debug.DrawLine(start, end, Color.red, 5);
      int j = 0;
      for(int i = 0; Mathf.Abs(i) <= Mathf.Abs(dy); i += stepY)
      {
        if(stepX > 0)
        {
          if((dx/dy)*i > (j+0.5f))
            j += stepX;
        }
        else
        {
          if((-dx/dy)*i > (j-0.5f))
            j += stepX;
        }
        map[tileA.row+i, tileA.col+j] = FLOOR;
      }
    }
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
  private void randomFillMap(int[,] map)
  {
    //if not random, seed will be empty thus returning same one each time
    if(isRandomSeed)
    {
      seed = (Time.time+randomizer).ToString();
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
        int neighbors = countWallNeighbors(map, i, j);
        if(neighbors > EQUAL_NEIGHBORS)
          map[i, j] = WALL;
        else if(neighbors < EQUAL_NEIGHBORS)
          map[i, j] = FLOOR;
      }
    }
  }

  //returns number of neighbors (all/half) that are walls
  private int countWallNeighbors(int[,] map, int tileRow, int tileCol)
  {
    int count = 0;
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
    return count;
  }
}
