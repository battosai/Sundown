using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownspersonClass : CharacterClass
{
    protected enum State {DEAD, IDLE, HOME, DEFEND, ALARM, HIDE, FLEE};
    protected readonly float ENTRANCE_RADIUS = 5f;
    protected State state;
    protected PlayerClass player;
    protected Hitbox hitbox;
    protected int nutrition;
    protected Building building;
    public void SetBuilding(Building building){this.building=building;}
    public void SetNutrition(int nutrition){this.nutrition=nutrition;}
    
    //called one time
    public void Init()
    {
        player = GameObject.Find("Player").GetComponent<PlayerClass>();
        hitbox = GetComponent<Hitbox>(); 
        SetType(CharacterClass.Type.TOWNSPERSON);
        base.Awake();
    }

    public override void Reset()
    {
        SetHealth(maxHealth);
        SetIsAlarmed(false);
        state = State.IDLE;
        Debug.Log("villager in node "+nodeID+" has map for node "+nodeID);
        nodeMap = PathFinding.Node.MakeNodeMap(World.wnodes[nodeID].map, nodeID);
        base.Reset();
    }

    //pass to coroutines to signify reaching end of path that leads home
    public void HomeCallback()
    {
        state = State.HOME;
    }
}