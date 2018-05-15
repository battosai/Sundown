using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: generates map procedurally that world references

public class MapGenerator : MonoBehaviour
{
  public static readonly int WIDTH = 100;
  public static readonly int HEIGHT = 100;
  public static readonly int EQUAL_NEIGHBORS = 4;

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
      Debug.Log("Button pressed.");
      generateMap();
    }
	}

  private void generateMap()
  {
    seed = "";
    map = new int[WIDTH, HEIGHT];
    randomFillMap();
    smoothMap();
  }

  //randomly fills map based on randomfillpercent
  private void randomFillMap()
  {
    //if not random, seed will be null thus returning same one each time
    if(isRandomSeed)
      seed = Time.time.ToString();
    System.Random rand = new System.Random(seed.GetHashCode());
    for(int i = 0; i < WIDTH; i++)
    {
      for(int j = 0; j < HEIGHT; j++)
      {
        if(i == 0 || j == 0 || i == WIDTH-1 || j == HEIGHT-1)
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
    for(int i = 0; i < WIDTH; i++)
    {
      for(int j = 0; j < HEIGHT; j++)
      {
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
        if(i < 0 || i >= WIDTH || j < 0 || j >= HEIGHT)
          continue;
        if(i == x && j == y)
          continue;
        count += map[i, j];
      }
    }
    return count;
  }

  //draw the map
  void OnDrawGizmos()
  {
    if(map != null)
    {
      for(int x = 0; x < WIDTH; x++)
      {
        for(int y = 0; y < HEIGHT; y++)
        {
          Gizmos.color = (map[x, y] == 1)? Color.black : Color.white;
          Vector3 pos = new Vector3(-WIDTH/2 + x + .5f, -HEIGHT/2 + y + .5f, 0);
          Gizmos.DrawCube(pos, Vector3.one);
        }
      }
    }
  }
}
