using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownspersonClass : CharacterClass
{
    public bool isAlarmed {get; private set;}
    protected enum State {IDLE, PATROL, INSPECT, DEFEND, ALARM, FLEE};
    protected State state;
    
    protected void init()
    {
        SetType(CharacterType.TOWNSPERSON);
    }
}