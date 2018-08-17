using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : TownspersonClass, IHitboxResponder
{
    //BUILDINGS STILL NEED TO BE SELECTED TO BE BARRACKS
    private readonly float RECOVERY_TIME = 5f;
    private readonly float ENTRANCE_RADIUS = 5f;
    private readonly int FLEE_HEALTH = 5;
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
                    if(isAlarmed)
                    {
                        state = State.DEFEND;
                        goto case State.DEFEND;
                    }
                    idleWalk(); //replace with patrol fnc later
                    break;
                case State.DEFEND:
                    if(health < FLEE_HEALTH)
                    {
                        state = State.FLEE;
                        fleeToBarracks();
                        goto case State.FLEE;
                    }
                    //should have a periodic alarm signal when fighting
                    break;
                case State.FLEE:
                    if(Vector2.Distance(floorPosition, barracks.entrance.transform.position) <= ENTRANCE_RADIUS)
                    {
                        rb.velocity = Vector2.zero;
                        time = Time.time;
                        state = State.HIDE;
                        rend.enabled = false;
                        pushBox.enabled = false;
                        SetIsAlarmed(false);
                        goto case State.HIDE;
                    }
                    break;
                case State.HIDE:
                    if(Time.time-time > RECOVERY_TIME)
                    {
                        SetHealth(maxHealth);
                        rend.enabled = true;
                        pushBox.enabled = true;
                        state = State.PATROL;
                        goto case State.PATROL;
                    }
                    break;
                default:
                    Debug.Log("[WARN]: Unknown Guard State "+state);
                    break;
            }
            base.Update();
        }
    }

    private void fleeToBarracks()
    {
        StartCoroutine(takePath(barracks.entrance.transform.position));
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
        state = State.PATROL;
        SetIsAlarmed(false);
        base.Reset();
    }
}