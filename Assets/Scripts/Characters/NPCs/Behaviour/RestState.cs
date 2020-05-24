using System;
using UnityEngine;

public class RestState : BaseState
{
    private static float REST_TIME = 5f;
    private StateMachine stateMachine;
    private float restTimer;
    private Vector2 restPosition; //position once char entered rest, so spawn them here at the end of rest

    public RestState(CharacterClass ch) : base(ch)
    {
        stateMachine = ch.stateMachine;

        stateMachine.OnStateChanged += BeginRest;
    }

    public override Type Tick()
    {
        if(restTimer > REST_TIME)
        {
            EndRest();
            return typeof(IdleState);
        }
        else
            restTimer += Time.deltaTime;
        return null;
    }

    private void EndRest()
    {
        character.SetHealth(character.maxHealth);
        rend.enabled = true;
        pushBox.enabled = true;
        trans.position = restPosition;
    }

    private void BeginRest(BaseState state)
    {
        if(state is RestState)
        {
            restTimer = 0f;
            restPosition = trans.position;
            rb.velocity = Vector2.zero;
            rend.enabled = false;
            pushBox.enabled = false;
            character.SetIsAlarmed(false);
        }
    }
}