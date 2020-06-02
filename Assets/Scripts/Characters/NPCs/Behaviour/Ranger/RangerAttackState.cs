using System;
using UnityEngine;

public class RangerAttackState : BaseState
{
    private static readonly int ATTACK_DMG = 2;
    private static readonly float ATTACK_RANGE = 100f;
    private static readonly float ATTACK_WIDTH = 2f;

    public RangerAttackState(Ranger ch) : base(ch)
    {
    }

    public override Type Tick()
    {
        RaycastHit2D hit = Physics2D.CircleCast(character.floorPosition, ATTACK_WIDTH, player.floorPosition - character.floorPosition, ATTACK_RANGE);
        if(hit.collider != null)
        {
            if(hit.collider.tag == "Player")
                player.hurtBox.Hurt(ATTACK_DMG);
        }
        return null;
    }
}