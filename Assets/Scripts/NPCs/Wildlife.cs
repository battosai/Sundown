using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wildlife : MonoBehaviour
{
    public int health {get; private set;}
    public int food {get; private set;}
    public float speed {get; private set;}
    public List<List<GameObject>> wildlife {get; private set;}
    public Rigidbody2D rb {get; private set;}

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        idleWalk();
    }

    private void idleWalk()
    {
        int action = Random.Range(0, 1);
        if(action == 0)
            rb.velocity = Vector2.zero;
        else
        {
            float horizontal = Random.Range(-speed, speed);
            float vertical = Random.Range(-speed, speed);
            rb.velocity = new Vector2(horizontal, vertical);
        }
    } 
}