using System;
using UnityEngine;

public class RangerAttackState : BaseState
{
    public RangerAttackState(Ranger ch) : base(ch)
    {

    }

    public override Type Tick()
    {
        // Debug.Log("Ranger is attacking!");
        // RaycastHit2D hit = Physics2D.CircleCast(floorPosition, ATTACK_WIDTH, player.floorPosition-floorPosition, ATTACK_RANGE);
        // if(hit.collider != null)
        // {
        //     if(hit.collider.tag == "Player")
        //         player.hurtBox.Hurt(ATTACK_DMG);
        // }
        return null;
    }
}