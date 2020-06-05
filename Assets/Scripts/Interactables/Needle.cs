using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Needle : MonoBehaviour 
{
	public static readonly float SPEED = 70f;
	private static readonly int DMG = 2;
	public Vector2 floorPosition {get; private set;}
	public Rigidbody2D rb {get; private set;}
	private float floorHeight;
	private Transform trans;
	private SpriteRenderer rend;
	private PlayerClass player;
	
	public void Awake()
	{
		trans = transform;
		rb = GetComponent<Rigidbody2D>();
		rend = GetComponent<SpriteRenderer>();
		player = GameObject.Find("Player").GetComponent<PlayerClass>();
	}

	public void Update()
	{
		setFloorHeight();	
	}

	public void OnCollisionEnter2D(Collision2D other)
	{
		switch(other.collider.tag)
		{
			case "Player":
				other.collider.GetComponent<Hurtbox>().Hurt(DMG, player);
				break;
			case "Wildlife":
				other.collider.GetComponent<Hurtbox>().Hurt(DMG);
				break;
			default:
				break;
		}
		this.gameObject.SetActive(false);
	}  
	
	private void setFloorHeight()
    {
		floorHeight = trans.position.y-(rend.bounds.size.y/2);
		trans.position = new Vector3(trans.position.x, trans.position.y, floorHeight);
		floorPosition = new Vector2(trans.position.x, floorHeight);
    }
}
