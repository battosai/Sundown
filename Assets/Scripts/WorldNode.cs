using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: handles node events (player interactions, hero interactions, etc)

public class WorldNode : World
{
  public int nodeID {get; private set;}
  public bool isActive {get; private set;}
  // private GameObject playerSpawn;
  // private GameObject playerExit;
  private Vector2 playerSpawn = new Vector2(0, 0);
  private Vector2 playerExit; = new Vector2(10, 0);

  public WorldNode(int nodeID)
  {
    this.nodeID = nodeID;
    this.isActive = true;
  }

  void Awake()
  {
    // playerSpawn = GameObject.Find(this.name + "/PlayerSpawn");
    // playerExit = GameObject.Find(this.name+"/PlayerExit");
  }

	// Update is called once per frame
	void Update()
	{

	}
}
