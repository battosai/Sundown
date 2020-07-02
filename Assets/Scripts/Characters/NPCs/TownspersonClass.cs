using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public abstract class TownspersonClass : CharacterClass
{
    protected readonly float ENTRANCE_RADIUS = 5f;
    protected PlayerClass player;
    public Hitbox hitbox {get; private set;}
    public Building building {get; private set;}
    public void SetBuilding(Building building){this.building=building;}

    protected abstract void InitializeStateMachine();
    
    //called one time
    public void Init()
    {
        player = GameObject.Find("Player").GetComponent<PlayerClass>();
        hitbox = GetComponent<Hitbox>(); 
        base.Awake();
        stateMachine = GetComponent<StateMachine>();
        InitializeStateMachine();
    }

    public override void Reset()
    {
        SetHealth(maxHealth);
        SetIsAlarmed(false);
        nodeMap = PathFinding.Node.MakeNodeMap(World.wnodes[nodeID].map, nodeID);
        base.Reset();
    }

    public override void Update()
    {
        if(health <= 0 && isAlive)
        {
            deathPrep();
            stateMachine.enabled = false;
        }
        UpdateAnimator();
        base.Update();
    }
}