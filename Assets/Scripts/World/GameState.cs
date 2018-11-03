using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: manages when game is ready to be progressed, or can't progress

public class GameState : MonoBehaviour
{
	public static readonly int DAYS_TO_WIN = 5;
	public static int day;
	private UIHandler uiHandler;
	private World world;
	private Arena arena;
	private PlayerClass player;
	private HeroClass hero;
	public void SetHero(HeroClass hero){this.hero = hero;}

	public void Awake()
	{
		uiHandler = GameObject.Find("UI").GetComponent<UIHandler>();
		arena = GameObject.Find("Arena").GetComponent<Arena>();
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
			world.gameObject.SetActive(false);
			arena.gameObject.SetActive(true);
			hero.PlaceInArena();
			hero.trans.position = hero.SetFloorPosition(arena.heroSpawn);
			player.trans.position = player.SetFloorPosition(arena.playerSpawn);
			Debug.Log("FIGHT THE HERO!");
		}
	}

	private void masterReset()
	{
		day = 0;
		if(!arena.isReady)
			arena.ConstructArena();
		arena.gameObject.SetActive(false);
		uiHandler.Reset();
		world.Reset();
		player.Reset();
		hero.Reset();
	}
}
