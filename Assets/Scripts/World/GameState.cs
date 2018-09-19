using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: manages when game is ready to be progressed, or can't progress

public class GameState : MonoBehaviour
{
	public static readonly int DAYS_TO_WIN = 5;
	private readonly float UNSET_TIME = -1f;
	private int day;
	private World world;
	private PlayerClass player;
	private HeroClass hero;
	public void SetHero(HeroClass hero){this.hero = hero;}

	public void Awake()
	{
		world = GameObject.Find("World").GetComponent<World>();
		player = GameObject.Find("Player").GetComponent<PlayerClass>();
	}

	// Use this for initialization
	public void Start()
	{
		masterReset();
	}

	// Update is called once per frame
	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.R))
			masterReset();
	}

	//called when player reaches exits
	public void NodeTransition(int currentNodeID)
	{
		day++;
		if(day < DAYS_TO_WIN)
		{
			//keep surviving!
			World.nodes[currentNodeID].SetActive(false);
			hero.Track(currentNodeID);	
			player.SetNodeID(day);
			World.nodes[day].SetActive(true);
			GameObject node = World.nodes[day];
			GameObject spawn = node.GetComponent<WorldNode>().playerSpawn;
			player.trans.position = player.SetFloorPosition(spawn.transform.position);
		}
		else
		{
			//time to fight the hero
			Debug.Log("FIGHT THE HERO!");
		}
	}

	private void masterReset()
	{
		day = 0;
		world.Reset();
		player.Reset();
		hero.Reset();
	}
}
