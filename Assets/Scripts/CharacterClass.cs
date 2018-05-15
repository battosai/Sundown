using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterClass : MonoBehaviour {
	public static readonly int BASE_SPEED = 10;
	public int nodeID {get; private set;}
	public float speed {get; private set;}
	public Transform trans;
	public Rigidbody2D rb;
	public SpriteRenderer rend;
	public Collider2D iColl;
	public void setNodeID(int id){nodeID = id;}

	void Awake()
	{
		trans = GetComponent<Transform>();
		rb = GetComponent<Rigidbody2D>();
		rend = GetComponent<SpriteRenderer>();
		iColl = GameObject.Find("InteractionCollider").GetComponent<Collider2D>();
	}

	void Start () {
		reset();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void reset() {
		speed = BASE_SPEED;
	}
}
