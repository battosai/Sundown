using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: interprets what to do when player uses the interact button

public class InteractionCollider : MonoBehaviour
{
	private CharacterClass player;

	void Awake()
	{
		player = GameObject.Find("Player").GetComponent<PlayerClass>();
	}

	// Use this for initialization
	void Start ()
	{
	}

	// Update is called once per frame
	void Update ()
	{

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		switch(other.gameObject.name)
		{
			case "PlayerExit":
				travel();
				break;
			default:
				break;
		}
	}

	private void travel()
	{
		if(player.nodeID < World.WORLD_SIZE-1)
		{
			Debug.Log("Traveling!");
			player.setNodeID(player.nodeID+1);
			GameObject node = World.nodes[player.nodeID];
			GameObject spawn = node.GetComponent<WorldNode>().playerSpawn;
			player.trans.position = spawn.GetComponent<Transform>().position;
		}
		else
		{
			Debug.Log("This is the last node");
		}
	}
}
