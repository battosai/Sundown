using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Villager : TownspersonClass
{
    private Building building;

    public void Start()
    {
        init();
    }

    public void Update()
    {
        switch(state)
        {
            case State.IDLE:
                break;
            case State.ALARM:
                break;
            default:
                Debug.Log("[Error] Unrecognized State: "+state);
                break;
        }
    }

    public void Reset()
    {
        state = State.IDLE;
    }
}