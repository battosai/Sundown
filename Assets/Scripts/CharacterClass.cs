using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//CONSIDER MAKING THIS CLASS NON MONOBEHAVIOUR SO WE CAN USE A NORMAL CONSTRUCTOR INSTEAD OF THE AWAKE FNC
public class CharacterClass : MonoBehaviour
{
	public readonly int BASE_SPEED = 10;
	public int nodeID {get; private set;}
	public float speed {get; private set;}
	public bool isLeft {get; private set;}
	public Transform trans {get; private set;}
	public Rigidbody2D rb {get; private set;}
	public SpriteRenderer rend {get; private set;}

	public void setNodeID(int id){nodeID = id;}
	public void setIsLeft(bool isLeft){this.isLeft = isLeft;}

	public virtual void Awake()
	{
		trans = GetComponent<Transform>();
		rb = GetComponent<Rigidbody2D>();
		rend = GetComponent<SpriteRenderer>();
	}

	public virtual void Reset()
	{
		speed = BASE_SPEED;
	}
}
