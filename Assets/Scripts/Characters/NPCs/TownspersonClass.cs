using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownspersonClass : CharacterClass
{
    protected enum State {DEAD, IDLE, INSPECT, DEFEND, ALARM, HIDE, FLEE};
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
        nodeMap = PathFinding.Node.MakeNodeMap(World.wnodes[nodeID].map, nodeID);
        base.Reset();
    }
}