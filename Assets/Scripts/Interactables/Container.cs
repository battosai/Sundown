using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Container : MonoBehaviour
{
    public Sprite full, empty;
    public bool isEmpty {get; private set;}
    public int gold {get; private set;} 
    public float floorHeight {get; private set;}
    private PlayerClass player;
    private SpriteRenderer rend;
    private Transform trans;

    public void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerClass>();
        trans = transform.parent;
        rend = transform.parent.GetComponent<SpriteRenderer>();
    }

    public void Start()
    {
        gold = Random.Range(0, 10);
        isEmpty = false;
        rend.sprite = full;
        setFloorHeight();
    }

    public void Update()
    {
        setFloorHeight();
    }

    public void Search()
    {
        if(isEmpty)
        {
            Debug.Log("[Error] Search being called on empty chest");
            return;
        }
        rend.sprite = empty;
        isEmpty = true;
        player.SetGold(player.gold+gold); 
        Debug.Log("player now has "+player.gold+" gold");
    }

    private void setFloorHeight()
    {
		floorHeight = trans.position.y-(rend.bounds.size.y/2);
		trans.position = new Vector3(trans.position.x, trans.position.y, floorHeight);
    }
}