using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour 
{
	private PlayerClass player;

	public void Awake()
	{
		player = GameObject.Find("Player").GetComponent<PlayerClass>();
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			Debug.Log("Player has been trapped!");
			gameObject.SetActive(false);
		}
	}
}
