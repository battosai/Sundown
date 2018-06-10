using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: handles node events (player interactions, hero interactions, etc)

public class WorldNode : MonoBehaviour
{
  public int nodeID {get; private set;}
  public bool isActive {get; private set;}
  public List<GameObject> collPool {get; private set;}
  public List<GameObject> wildlifePool {get; private set;}
  public GameObject playerSpawn {get; private set;}
  public GameObject playerExit {get; private set;}
  public MeshFilter meshFilter {get; private set;}

  public void setNodeID(int nodeID){this.nodeID = nodeID;}
  public void addCollPool(GameObject coll){this.collPool.Add(coll);}
  public void addWildlifePool(GameObject wildlife){this.wildlifePool.Add(wildlife);}

  void Awake()
  {
    playerSpawn = GameObject.Find(this.name + "/PlayerSpawn");
    playerExit = GameObject.Find(this.name + "/PlayerExit");
    meshFilter = GetComponent<MeshFilter>();
    collPool = new List<GameObject>();
    wildlifePool = new List<GameObject>();
  }

  void Start()
  {
    reset();
  }

	// Update is called once per frame
	void Update()
	{

	}

  public void reset()
  {
    isActive = true;
  }
}
