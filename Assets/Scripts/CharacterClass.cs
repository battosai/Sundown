using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterClass : MonoBehaviour {
	public static readonly int BASE_SPEED = 10;
	public static int nodeID {get; private set;}
	public static float speed {get; private set;}
	public static Transform trans;
	public static Rigidbody2D rb;
	public static SpriteRenderer rend;
	public static Collider2D iColl;
	public static void setNodeID(int id){nodeID = id;}

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

	public static void reset() {
		speed = BASE_SPEED;
	}
}
