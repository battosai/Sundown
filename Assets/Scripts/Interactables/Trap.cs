using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour 
{
	public static readonly float TRAP_TIME = 2f;
	private PlayerClass player;

	public delegate void Trapped();
	public static event Trapped OnTrapped;

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
			// player.rb.velocity = Vector2.zero;
			// player.BecomeTrapped();
			OnTrapped.Invoke();
		}
	}
}
