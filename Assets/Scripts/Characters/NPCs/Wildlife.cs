using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Game;

public class Wildlife : CharacterClass
{
    public int clue {get; private set;}
    protected static PlayerClass player;
    protected int nutrition;
    public void SetClue(int clue){this.clue = clue;}

    public static event Action OnPlayerWildlifeHeal;

    //called one time
    public void Init()
    {
        if(player == null)
            player = GameObject.Find("Player").GetComponent<PlayerClass>();
        stateMachine = GetComponent<StateMachine>();
        SetType(CharacterType.WILDLIFE);
        base.Awake();
        InitializeStateMachine();
    }

    private void InitializeStateMachine()
    {
        Dictionary<Type, BaseState> states = new Dictionary<Type, BaseState>()
        {
            {typeof(IdleState), new IdleState(this)},
            {typeof(FleeState), new FleeState(this)}
        };
        stateMachine.SetStates(states);
    }

    public override void Update()
    {
        if(health <= 0 && isAlive)
        {
            deathPrep();
            stateMachine.enabled = false;
            player.SetHealth(player.health+nutrition);
            OnPlayerWildlifeHeal?.Invoke();
            WorldNode wnode = World.nodes[nodeID].GetComponent<WorldNode>();
            wnode.SetClues(wnode.clues+clue);
            Debug.Log("WorldNode now has "+wnode.clues+" clues");
        }
        base.Update();
    }

    public override void Reset()
    {
        rend.sprite = alive;
        SetHealth(maxHealth);
        base.Reset();
    }
}