using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownspersonClass : CharacterClass
{
    protected bool isAlive;
    protected enum State {DEAD, IDLE, PATROL, INSPECT, DEFEND, ALARM, HIDE, FLEE};
    protected State state;
    
    protected void init()
    {
        SetType(CharacterType.TOWNSPERSON);
    }
}