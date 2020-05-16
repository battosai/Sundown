using System;
using UnityEngine;

public class IdleState : BaseState
{
    private static float CHANGE_ACTION_TIME = 1f;
    private bool isHostile;
    private float actionTimer;

    public IdleState(CharacterClass character) : base(character)
    {
        this.isHostile = (character is Guard || character is HeroClass);
    }
    public override Type Tick()
    {
        if(character.isAlarmed)
            return isHostile ? typeof(ChaseState) : typeof(FleeState);
        actionTimer += Time.deltaTime;
        if(actionTimer > CHANGE_ACTION_TIME)
        {
            int action = UnityEngine.Random.Range(0, 5);
            if(action == 0)
            {
                float horizontal = UnityEngine.Random.Range(-character.speed, character.speed);
                float vertical = UnityEngine.Random.Range(-character.speed, character.speed);
                rb.velocity = new Vector2(horizontal, vertical);
            }
            else
                rb.velocity = Vector2.zero;
            actionTimer = 0f;
        }
        return null;
    }
}