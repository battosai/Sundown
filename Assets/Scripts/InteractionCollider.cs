using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: interprets what to do when player uses the interact button

public class InteractionCollider : MonoBehaviour
{

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
		Debug.Log("Traveling!");
	}
}
