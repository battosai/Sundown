using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wildlife : CharacterClass
{
    public Sprite alive;
    public Sprite dead;
    public int clue {get; private set;}
    public float blood {get; private set;}
    protected enum State {IDLE, FLEE, DEAD};
    protected PlayerClass player;
    protected State state;
    protected int nutrition;
    private readonly float FLEE_SPEED = 40f;
    private readonly float FLEE_DISTANCE = 100f;
    public void SetNutrition(int nutrition){this.nutrition=nutrition;}
    public void SetClue(int clue){this.clue = clue;}
    public void SetBlood(float blood){this.blood=blood;}

    //called one time
    public void Init()
    {
        player = GameObject.Find("Player").GetComponent<PlayerClass>();
        SetType(CharacterType.WILDLIFE);
        base.Awake();
    }

    public override void Update()
    {
        setFloorHeight();
        if(health <= 0)
            state = State.DEAD;
        if(isAlive)
        {
            switch(state)
            {
                case State.IDLE:
                    if(isAlarmed)
                    {
                        SetSpeed(FLEE_SPEED);
                        state = State.FLEE;
                        goto case State.FLEE;
                    }
                    if(Time.time - time > 1f)
                    {
                        time = Time.time;
                        idleWalk();
                    }
                    break;
                case State.FLEE:
                    if(Vector2.Distance(alarmPoint, floorPosition) > FLEE_DISTANCE)
                    {
                        SetSpeed(BASE_SPEED);
                        SetIsAlarmed(false);
                        state = State.IDLE;
                        goto case State.IDLE;
                    }
                    rb.velocity = new Vector2(-1f, -1f)*PathFinding.GetVelocity(floorPosition, alarmPoint, speed);
                    break;
                case State.DEAD:
                    SetIsAlive(false);
                    rend.sprite = dead;
                    rb.velocity = Vector2.zero;
                    player.SetFood(player.food+nutrition);
                    Debug.Log("Player now has "+player.food+" food");
                    WorldNode wnode = World.nodes[nodeID].GetComponent<WorldNode>();
                    wnode.SetClues(wnode.clues+clue);
                    Debug.Log("WorldNode now has "+wnode.clues+" clues");
                    break;
                default:
                    break;
            }
            base.Update();
        }
    }

    public override void Reset()
    {
        rend.sprite = alive;
        state = State.IDLE;
        SetHealth(maxHealth);
        base.Reset();
    }
}