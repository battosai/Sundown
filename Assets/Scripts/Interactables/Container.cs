using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Container : MonoBehaviour
{
    public bool isEmpty;
    public int gold {get; private set;} 
    public float floorHeight {get; private set;}
    private SpriteRenderer rend;
    private Transform trans;

    public void Awake()
    {
        trans = transform.parent;
        rend = transform.parent.GetComponent<SpriteRenderer>();
    }

    public void Start()
    {
        gold = Random.Range(0, 10);
        isEmpty = false;
        setFloorHeight();
    }

    private void setFloorHeight()
    {
		floorHeight = trans.position.y-(rend.bounds.size.y/2);
		trans.position = new Vector3(trans.position.x, trans.position.y, floorHeight);
    }
}