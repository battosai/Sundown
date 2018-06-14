using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wildlife : MonoBehaviour
{
    public int health {get; private set;}
    public int food {get; private set;}
    public float speed {get; private set;}
    public bool isHidden {get; private set;}
    public Rigidbody2D rb {get; private set;}
    private enum State {CALM, ALERTED};
    private State state;
    private float time;

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Start()
    {
        speed = 20f;
        time = Time.time;
        state = State.CALM;
    }

    public void Update()
    {
        switch(state)
        {
            case State.CALM:
                if(Time.time - time > 1f)
                {
                    time = Time.time;
                    idleWalk();
                }
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