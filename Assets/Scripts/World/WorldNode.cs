using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: handles node events (player interactions, hero interactions, etc)

public class WorldNode : MonoBehaviour
{
  public int nodeID {get; private set;}
  public int width {get; private set;}
  public int height {get; private set;}
  public int clues {get; private set;}
  public int[,] map {get; private set;}
  public bool isActive {get; private set;}
  public List<GameObject> collPool {get; private set;}
  public List<GameObject> bigAnimalPool {get; private set;}
  public List<GameObject> smallAnimalPool {get; private set;}
  public List<GameObject> buildingPool {get; private set;}
  public GameObject playerSpawn {get; private set;}
  public GameObject playerExit {get; private set;}
  public MeshFilter meshFilter {get; private set;}

  public void SetNodeID(int nodeID){this.nodeID = nodeID;}
  public void SetWidth(int width){this.width = width;}
  public void SetHeight(int height){this.height = height;}
  public void SetClues(int clues){this.clues = clues;}
  public void SetMap(int[,] map){this.map = map;}
  public void AddPoolObject(GameObject obj, List<GameObject> pool){pool.Add(obj);}

  void Awake()
  {
    playerSpawn = GameObject.Find(this.name + "/PlayerSpawn");
    playerExit = GameObject.Find(this.name + "/PlayerExit");
    meshFilter = GetComponent<MeshFilter>();
    collPool = new List<GameObject>();
    bigAnimalPool = new List<GameObject>();
    smallAnimalPool = new List<GameObject>();
    buildingPool = new List<GameObject>();
  }

  //called by parent class World when being reset
  public void ParentReset()
  {
    isActive = true;
    clues = 0;
    playerExit.SetActive(false);
  }

  public void ExitFound()
  {
    playerExit.SetActive(true);
  }
}
