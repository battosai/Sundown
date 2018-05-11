using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: generates map procedurally that world references

public class MapGenerator : MonoBehaviour
{
  public static readonly int width = 50;
  public static readonly int height = 50;
  public static int[,] map {get; private set;}
  private string seed;
  [Range(0, 100)]
  public int randomFillPercent;
  [Range(1, 8)]
  public int smoothingRate;
  public boolean isRandomSeed;

	void Awake()
	{
	}

	// Use this for initialization
	void Start()
	{
    generateMap();
	}

	// Update is called once per frame
	void Update()
	{
	}

  private void generateMap()
  {
    map = new int[width, height];
    randomFillMap();
    smoothMap();
  }

  //randomly fills map based on randomfillpercent
  private void randomFillMap()
  {
    //if not random, seed will be null thus returning same one each time
    if(isRandomSeed)
      seed = Time.time.ToString();
    System.Random rand = new Random(seed.GetHashCode());
    for(int i = 0; i < width; i++)
    {
      for(int j = 0; j < height; j++)
      {
        if(i == 0 || j == 0 || i == width-1 || j == height-1)
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
    for(int i = 0; i < width; i++)
    {
      for(int j = 0; j < height; j++)
      {
        int neighbors = countWallNeighbors(i, j);
        if(neighbors > smoothingRate)
          map[i, j] = 1;
        else if(neighbors < smoothingRate)
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
        if(i == x && j == y)
          continue;
        count += map[i, j];
      }
    }
    return count;
  }
}
