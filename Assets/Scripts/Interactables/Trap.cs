using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour 
{
	public static readonly float IMMOBILE_TIME = 2f;
	private static readonly float TRAP_DURATION = 6f;
	private PlayerClass player;
	private float durationTimer;

	public delegate void Trapped();
	public static event Trapped OnTrapped;

	public void Awake()
	{
		player = GameObject.Find("Player").GetComponent<PlayerClass>();
	}

	public void Start()
	{
		durationTimer = 0f;
	}

	public void Update()
	{
		if(durationTimer > TRAP_DURATION)
		{
			durationTimer = 0f;
			this.gameObject.SetActive(false);
		}
		else
			durationTimer += Time.deltaTime;
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			OnTrapped.Invoke();
			gameObject.SetActive(false);
		}
	}
}
