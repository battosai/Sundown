using System;
using UnityEngine;

public class ChaseState : BaseState
{
	private PlayerClass player;

	public ChaseState(CharacterClass ch) : base(ch)
	{
		this.player = GameObject.Find("Player").GetComponent<PlayerClass>();
	}

    public override Type Tick()
    {
		rb.velocity = Vector2.zero;
		float dist = Vector2.Distance(character.floorPosition, player.floorPosition);
        //check if still in aggro range
        if(dist > Guard.AGGRO_LEASH)
            return typeof(IdleState);
		if(dist <= Guard.ATTACK_RANGE)
			return typeof(AttackState);

        //chase that mofo
		rb.velocity = PathFinding.GetVelocity(character.floorPosition, player.floorPosition, character.speed);

        return null;
    }
}
