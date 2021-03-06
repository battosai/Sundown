using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Container : MonoBehaviour
{
    public Sprite full, empty;
    public bool isEmpty {get; private set;}
    public float floorHeight {get; private set;}
    private bool hasMap;
    private PlayerClass player;
    private SpriteRenderer rend;
    private Transform trans;
    public void SetHasMap(bool hasMap){this.hasMap=hasMap;}

	//event for map discovery
	public delegate void FoundMap();
	public static event FoundMap OnFoundMap;

    public void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerClass>();
        trans = transform.parent;
        rend = transform.parent.GetComponent<SpriteRenderer>();
    }

    public void Start()
    {
        isEmpty = false;
        rend.sprite = full;
        setFloorHeight();
    }

    public void Search()
    {
        if(isEmpty)
        {
            Debug.LogError("Search being called on empty chest");
            return;
        }
        if(hasMap)
        {
            Debug.Log("Exit has been revealed!");
            SetHasMap(false);
            OnFoundMap.Invoke();
        }
        rend.sprite = empty;
        isEmpty = true;
    }

    private void setFloorHeight()
    {
		floorHeight = trans.position.y-(rend.bounds.size.y/2);
		trans.position = new Vector3(trans.position.x, trans.position.y, floorHeight);
    }
}