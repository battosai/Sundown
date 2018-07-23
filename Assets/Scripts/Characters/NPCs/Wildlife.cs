using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wildlife : CharacterClass
{
    public Sprite alive;
    public Sprite dead;
    public bool isDead {get; private set;}
    public int maxHealth {get; private set;}
    public int nutrition {get; private set;}
    public int clue {get; private set;}
    public float blood {get; private set;}
    protected enum State {CALM, ALERTED, DEAD};
    protected PlayerClass player;
    protected State state;
    protected float time;
    public void SetMaxHealth(int maxHealth){this.maxHealth=maxHealth;}
    public void SetNutrition(int nutrition){this.nutrition=nutrition;}
    public void SetClue(int clue){this.clue = clue;}
    public void SetBlood(float blood){this.blood=blood;}

    public void Init()
    {
        base.Awake();
        player = GameObject.Find("Player").GetComponent<PlayerClass>();
        SetType(CharacterType.WILDLIFE);
    }

    public void Update()
    {
        setFloorHeight();
        if(health <= 0)
            state = State.DEAD;
        if(!isDead)
            switch(state)
            {
                case State.CALM:
                    if(Time.time - time > 1f)
                    {
                        time = Time.time;
                        idleWalk();
                    }
                    break;
                case State.DEAD:
                    isDead = true;
                    rend.sprite = dead;
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
    }

    private void idleWalk()
    {
        int action = Random.Range(0, 5);
        if(action == 0)
        {
            float horizontal = Random.Range(-speed, speed);
            float vertical = Random.Range(-speed, speed);
            rb.velocity = new Vector2(horizontal, vertical);
            SetIsLeft(horizontal < 0);
        }
        else
            rb.velocity = Vector2.zero;
    } 

    public override void Reset()
    {
        base.Reset();
        rend.sprite = alive;
        isDead = false;
        time = Time.time;
        state = State.CALM;
        SetHealth(maxHealth);
    }
}