using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: handles node events (player interactions, hero interactions, etc)

public class WorldNode : World
{
  //public, only writeable in-class
  public int nodeID {get; private set;}
  public bool isActive {get; private set;}

  public WorldNode(int nodeID)
  {
    this.nodeID = nodeID;
    this.isActive = true;
  }

	// Update is called once per frame
	void Update()
	{

	}

}
