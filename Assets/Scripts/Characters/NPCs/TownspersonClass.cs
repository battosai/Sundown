using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class TownspersonClass : CharacterClass
{
    protected enum State {DEAD, IDLE, HOME, DEFEND, ALARM, HIDE, FLEE};
    protected readonly float ENTRANCE_RADIUS = 5f;
    protected State state;
    protected PlayerClass player;
    public Hitbox hitbox {get; private set;}
    public Building building {get; private set;}
    public void SetBuilding(Building building){this.building=building;}
    
    //called one time
    public void Init()
    {
        player = GameObject.Find("Player").GetComponent<PlayerClass>();
        hitbox = GetComponent<Hitbox>(); 
        SetType(CharacterType.TOWNSPERSON);
        base.Awake();
    }

    public override void Reset()
    {
        SetHealth(maxHealth);
        SetIsAlarmed(false);
        state = State.IDLE;
        nodeMap = PathFinding.Node.MakeNodeMap(World.wnodes[nodeID].map, nodeID);
        base.Reset();
    }

    //pass to coroutines to signify reaching end of path that leads home
    public void HomeCallback()
    {
        state = State.HOME;
    }

    public void WaitCallback()
    {
        StartCoroutine(wait(0.2f));
    }
}