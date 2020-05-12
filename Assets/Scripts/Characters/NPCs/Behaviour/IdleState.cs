using System;
using UnityEngine;

public class IdleState : BaseState
{
    private bool isHostile;

    public IdleState(CharacterClass character) : base(character)
    {
        this.isHostile = (character is Guard || character is HeroClass);
    }
    public override Type Tick()
    {
        if(character.isAlarmed)
            return isHostile ? typeof(ChaseState) : typeof(FleeState);

        //behaviour of CharacterClass.idleWalk()
        int action = UnityEngine.Random.Range(0, 5);
        if(action == 0)
        {
            float horizontal = UnityEngine.Random.Range(-character.speed, character.speed);
            float vertical = UnityEngine.Random.Range(-character.speed, character.speed);
            character.rb.velocity = new Vector2(horizontal, vertical);
        }
        else
            rb.velocity = Vector2.zero;

        return null;
    }
}