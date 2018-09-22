using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
	public Vector2 playerSpawn {get; private set;}
	public Vector2 heroSpawn {get; private set;}
	private PlayerClass player;
	private HeroClass hero;	

	public void Awake()
	{
		player = GameObject.Find("Player").GetComponent<PlayerClass>();
		hero = GameObject.Find("Hero").GetComponent<HeroClass>();
		playerSpawn = transform.Find("PlayerSpawn").transform.position;
		heroSpawn = transform.Find("HeroSpawn").transform.position;
	}
}
