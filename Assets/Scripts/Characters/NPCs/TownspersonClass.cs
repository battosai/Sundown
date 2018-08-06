using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownspersonClass : CharacterClass
{
    protected bool isAlive;
    protected bool isAlarmed;
    protected enum State {IDLE, PATROL, INSPECT, DEFEND, ALARM, FLEE};
    protected State state;
    
    protected void init()
    {
        SetType(CharacterType.TOWNSPERSON);
    }
}