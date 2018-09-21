using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
	public Vector2 playerSpawn {get; private set;}
	public Vector2 heroSpawn {get; private set;}
	private readonly Vector2 PLAYER_SPAWN = new Vector2(-5f, 0f);
	private readonly Vector2 HERO_SPAWN = new Vector2(5f, 0f);
	private PlayerClass player;
	private HeroClass hero;	

	public void Awake()
	{
		player = GameObject.Find("Player").GetComponent<PlayerClass>();
		hero = GameObject.Find("Hero").GetComponent<HeroClass>();
	}

	public void Start()
	{
		playerSpawn = PLAYER_SPAWN;
		heroSpawn = HERO_SPAWN;
	}
}
