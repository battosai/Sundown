using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: handles node events (player interactions, hero interactions, etc)

public class WorldNode : MonoBehaviour
{
  public int nodeID {get; private set;}
  public int width {get; private set;}
  public int height {get; private set;}
  public int[,] map {get; private set;}
  public int[,] reservedMap {get; private set;}
  public bool isActive {get; private set;}
  public List<GameObject> collPool {get; private set;}
  public List<GameObject> wildlifePool {get; private set;}
  public GameObject playerSpawn {get; private set;}
  public GameObject playerExit {get; private set;}
  public MeshFilter meshFilter {get; private set;}

  public void SetNodeID(int nodeID){this.nodeID = nodeID;}
  public void SetWidth(int width){this.width = width;}
  public void SetHeight(int height){this.height = height;}
  public void SetMap(int[,] map){this.map = map;}
  public void SetReservedMap(int[,] reservedMap){this.reservedMap = reservedMap;}
  public void AddPoolObject(GameObject obj, List<GameObject> pool){pool.Add(obj);}

  void Awake()
  {
    playerSpawn = GameObject.Find(this.name + "/PlayerSpawn");
    playerExit = GameObject.Find(this.name + "/PlayerExit");
    meshFilter = GetComponent<MeshFilter>();
    collPool = new List<GameObject>();
    wildlifePool = new List<GameObject>();
  }

	// Update is called once per frame
	void Update()
	{

	}

  //called by parent class World when being reset
  public void ParentReset()
  {
    isActive = true;
  }
}
