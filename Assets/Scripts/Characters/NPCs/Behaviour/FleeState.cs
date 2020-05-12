using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState : BaseState
{
    private static float PATHFINDING_TOLERANCE = 1f;
    private PlayerClass player;
    private List<Vector2> path;

    public FleeState(CharacterClass ch) : base(ch)
    {
		this.player = GameObject.Find("Player").GetComponent<PlayerClass>();
        path = new List<Vector2>();
    }

    public override Type Tick()
    {
        rb.velocity = Vector2.zero;
        //if wildlife, run away from player
        if(character is Wildlife)
        {
            rb.velocity = new Vector2(-1f, -1f)*PathFinding.GetVelocity(character.floorPosition, player.floorPosition, character.speed*2);
        }
        //if townie, run home
        else
        {
            if(path.Count > 0)
            {
                if(Vector2.Distance(character.floorPosition, path[0]) <= PATHFINDING_TOLERANCE)
                    path.RemoveAt(0);
                rb.velocity = PathFinding.GetVelocity(character.floorPosition, path[0], character.speed);
            }
            else
            {
                // once path is done, switch to a different state
            }
        }
        return null;
    }

    //call this right when state is set to Flee
    public void GetPath()
    {
		path = PathFinding.AStarJump(character.floorPosition, ((TownspersonClass)character).building.entrance.transform.position, character.nodeMap, character.nodeID);
    }
}