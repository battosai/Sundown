using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownspersonClass : CharacterClass
{
    protected enum State {DEAD, IDLE, PATROL, INSPECT, DEFEND, ALARM, HIDE, FLEE};
    protected State state;
    protected PlayerClass player;
    protected int nutrition;
    protected bool isAlive;
    public void SetNutrition(int nutrition){this.nutrition=nutrition;}
    
    //called one time
    public void Init()
    {
        player = GameObject.Find("Player").GetComponent<PlayerClass>();
        SetType(CharacterType.TOWNSPERSON);
        base.Awake();
    }

    public override void Reset()
    {
        SetHealth(maxHealth);
        nodeMap = PathFinding.Node.MakeNodeMap(World.wnodes[nodeID].map, nodeID);
        base.Reset();
    }
}