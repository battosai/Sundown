using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bed : MonoBehaviour
{
    public float floorHeight {get; private set;}
    private PlayerClass player;
    private SpriteRenderer rend;
    private Transform trans;

    public void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerClass>();
        trans = transform;
        rend = transform.GetComponent<SpriteRenderer>();
    }

    public void Start()
    {
        setFloorHeight();
    }

    public void Update()
    {
        // setFloorHeight();
    }

    public void Search()
    {
    }

    private void setFloorHeight()
    {
		floorHeight = trans.position.y-(rend.bounds.size.y/2);
		trans.position = new Vector3(trans.position.x, trans.position.y, floorHeight);
    }
}