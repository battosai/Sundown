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
                    if(Time.time - time > 1f)
                    {
                        time = Time.time;
                        idleWalk();
                    }
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
                    // this.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
            base.Update();
        }
    }

    private void idleWalk()
    {
        int action = Random.Range(0, 5);
        if(action == 0)
        {
            float horizontal = Random.Range(-speed, speed);
            float vertical = Random.Range(-speed, speed);
            rb.velocity = new Vector2(horizontal, vertical);
        }
        else
            rb.velocity = Vector2.zero;
    } 

    public override void Reset()
    {
        rend.sprite = alive;
        state = State.IDLE;
        SetHealth(maxHealth);
        base.Reset();
    }
}