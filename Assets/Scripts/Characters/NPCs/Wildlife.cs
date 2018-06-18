using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wildlife : CharacterClass
{
    public readonly int nutrition = 5;
    public bool isHidden {get; private set;}
    private enum State {CALM, ALERTED, DEAD};
    private State state;
    private float time;

    public void Start()
    {
        SetHealth(5);
        time = Time.time;
        state = State.CALM;
    }

    public void Update()
    {
        if(health <= 0)
            state = State.DEAD;
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
                this.gameObject.SetActive(false);
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
        }
        else
            rb.velocity = Vector2.zero;
    } 
}