using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger : HeroClass, IHitboxResponder
{
    private readonly int MASTERY = 2;
    private readonly float AGGRO_RANGE = 100f;
	private Vector2 INTERACT_SIZE = new Vector2(50f, 50f);

	public override void Awake()
	{
		player = GameObject.Find("Player").GetComponent<PlayerClass>();
        hitBox = GetComponent<Hitbox>();
		base.Awake();
	}

    public void Start()
    {
        init();
        hitBox.SetResponder(this); 
        gameState.SetHero(this);
    }

    public override void Update()
    {
        if(isPresent)
        {
            //test
            // float mapWidth = MapGenerator.COLS*MeshGenerator.SQUARE_SIZE;
    	    // float mapHeight = MapGenerator.ROWS*MeshGenerator.SQUARE_SIZE;
            // world.DisplayFloor();
            // int[] rowcol = World.NearestMapPair(trans.position, nodeID);
            // Vector2 coords = World.ConvertMapToWorld(rowcol[0], rowcol[1], nodeID);
            // Debug.DrawLine(new Vector3(coords.x-2f, coords.y,0f), new Vector3(coords.x+2f, coords.y, 0f), Color.cyan, 1f);
            // if(PlayerInput.Space)
            // {
            //     List<Vector2> path = PathFinding.AStarJump(floorPosition, player.floorPosition, nodeMap, nodeID);
            //     float markerSize = 2f;
            //     float duration = 1f;
            //     for(int i = 1; i < path.Count; i++)
            //     {
            //         Debug.DrawLine((Vector3)path[i-1], (Vector3)path[i], Color.cyan, duration);
            //     }
            // }
            // if(playerSpotted())
            // {
            //     Debug.Log("YOU'VE BEEN SPOTTED!");
            // }
            //end test
            switch(state)
            {
                case State.IDLE:
                    if(isAlarmed)
                    {
                        Debug.Log("RANGER IS DOING AN INSPECT");
                        StartCoroutine(takePath(alarmPoint));
                        state = State.INSPECT;
                        goto case State.INSPECT;
                        //maybe also add a trigger to constantly look for the player
                    } 
                    if(Time.time-time > 1f)
                    {
                        time = Time.time;
                        idleWalk();
                    }
                    break;
                case State.INSPECT:
                    interactCheck();
                    break;
                default:
                    Debug.Log("[Error] Unknown Ranger Action: "+state);
                    break;
            }
        }
        base.Update();
        setFloorHeight();
    }

    public override void Track(int nodeID)
    {
        Debug.Log("Ranger Mastery Track!"); 
        WorldNode wnode = World.wnodes[nodeID];
        SetLeads(leads+wnode.clues*tracking*MASTERY);
        presentInNode(false);
        if(leads >= PLAYER_FOUND)
        {
            Debug.Log("Ranger has found your current location!");
            presentInNode(true, nodeID+1);
            SetLeads(0);
        }
        Debug.Log("Ranger now has "+leads+" leads");
    }

    public override void Reset()
    {
        SetLeads(0f);
        presentInNode(true, 0);
        tracking = 0.8f;
        visionRange = 100f;
        base.Reset();
    }

    public void Hit(Collider2D other, Action action)
    {
        switch(action)
        {
            case Action.INTERACT:
                interact(other);
                break;
            case Action.ATTACK:
                break;
            default:
                Debug.Log("[Error] Unrecognized Ranger Action");
                break;
        }
    }

    private void interactCheck()
    {
        hitBox.mask.useTriggers = false;
		hitBox.SetAction(Action.INTERACT);
		hitBox.SetOffset(floorPosition);
		hitBox.SetSize(INTERACT_SIZE);
		hitBox.StartCheckingCollision();
		hitBox.CheckCollision();
		hitBox.StopCheckingCollision();
    }

    //effectively the ranger's "scout" ability
    private void interact(Collider2D other)
    {
       
    }
}