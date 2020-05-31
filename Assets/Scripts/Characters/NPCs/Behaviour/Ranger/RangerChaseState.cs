using System;
using System.Collections.Generic;
using UnityEngine;

public class RangerChaseState : BaseState
{
    private StateMachine stateMachine;
    private List<Vector2> path;

    public RangerChaseState(Ranger ch) : base(ch)
    {
        stateMachine = ch.stateMachine;
        path = new List<Vector2>();
    }

    public override Type Tick()
    {
        //if valid LOS, attack
        if(HasLineOfSight())
        {
            path.Clear();
            return typeof(RangerAttackState);
        }
        
        //else move towards player while checking
        if(path.Count == 0)
            path = PathFinding.AStarJump(character.floorPosition, player.floorPosition, character.nodeMap, character.nodeID);
        else
        {
            rb.velocity = PathFinding.GetVelocity(character.floorPosition, path[0], character.speed);
            if(((Vector2)(character.floorPosition - path[0])).sqrMagnitude <= PathFinding.TOLERANCE)
                path.RemoveAt(0);

            //if we reach where the end of path without LOS, go to idle
            if(path.Count == 0)
                return typeof(RangerIdleState);
        }
        return null;
    }

    private bool HasLineOfSight()
    {
        Vector2 direction = player.floorPosition-character.floorPosition;
        if((character.isLeft && Mathf.Sign(direction.x) < 0) || (!character.isLeft && Mathf.Sign(direction.x) > 0))
        {
            RaycastHit2D vision = Physics2D.Raycast(character.floorPosition, player.floorPosition - character.floorPosition, Ranger.VISION_RANGE);
			if(vision.collider != null)
			{
				if(vision.collider.gameObject.name == "Player")
					return true;
			}
        }
        return false;
    }
}