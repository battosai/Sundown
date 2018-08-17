using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : TownspersonClass, IHitboxResponder
{
    private Building barracks;
    public void SetBase(Building barracks){this.barracks=barracks;}

    public override void Awake()
    {
        Reset();
    }    

    public void Start()
    {
        hitbox.SetResponder(this);
    }

    public override void Update()
    {
        if(isAlive)
        {
            switch(state)
            {
                case State.DEAD:
                    SetIsAlive(false);
                    rb.velocity = Vector2.zero;
                    break;
                case State.PATROL:
                    break;
                case State.DEFEND:
                    break;
                case State.FLEE:
                    break;
                case State.HIDE:
                    break;
                default:
                    Debug.Log("[WARN]: Unknown Guard State "+state);
                    break;
            }
        }
    }

    public void Hit(Collider2D other, Action action)
    {
        switch(action)
        {
            case Action.ATTACK:
                break;
            case Action.INTERACT:
                break;
            case Action.ALARM:
                break;
            default:
                Debug.Log("[WARN]: Unknown Guard Action "+action);
                break;
        }
    }

    public override void Reset()
    {
        state = State.IDLE;
        SetIsAlarmed(false);
        base.Reset();
    }
}