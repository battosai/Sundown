using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType {PLAYER, HERO, WILDLIFE, TOWNSPERSON};
public class CharacterClass : MonoBehaviour
{
	public readonly float BASE_SPEED = 20f;
	public CharacterType type {get; private set;}
	public int nodeID {get; private set;}
	public int health {get; private set;}
	public float speed {get; private set;}
	public float floorHeight {get; private set;}
	public Vector2 floorPosition {get; private set;}
	public bool isLeft {get; private set;}
	public GameState gameState {get; private set;}
	public World world {get; private set;}
	public Transform trans {get; private set;}
	public Rigidbody2D rb {get; private set;}
	public SpriteRenderer rend {get; private set;}
	protected PathFinding.Node[,] nodeMap;
	public void SetType(CharacterType type){this.type = type;}
	public void SetNodeID(int id){nodeID = id;}
	public void SetIsLeft(bool isLeft){this.isLeft = isLeft;}
	public void SetHealth(int health){this.health = health;}
	public void SetSpeed(int speed){this.speed = speed;}

	public virtual void Awake()
	{
		gameState = GameObject.Find("GameState").GetComponent<GameState>();
		world = GameObject.Find("World").GetComponent<World>(); 
		trans = GetComponent<Transform>();
		rb = GetComponent<Rigidbody2D>();
		rend = GetComponent<SpriteRenderer>();
	}

	//default Update for characters
	public virtual void Update()
	{
		Vector2 velocity = rb.velocity;
		if(velocity.x > 0)
			isLeft = false;
		else if(velocity.x < 0)
			isLeft = true;
		rend.flipX = !isLeft;
	}

	public virtual void Reset()
	{
		speed = BASE_SPEED;
	}

	public virtual void UpdateAnimator()
	{
		Debug.Log("[WARN] "+gameObject.name+" has no proper UpdateAnimator() method");
	}
	
	//sets position so that floor position is at target
	public Vector2 SetFloorPosition(Vector2 target)
	{
		float yOffset = rend.bounds.size.y/2;
		return new Vector2(target.x, target.y+yOffset);
	}

	//sets floor height (bottom of sprite), used for collisions and rendering orders
	protected void setFloorHeight()
	{
		floorHeight = trans.position.y-(rend.bounds.size.y/2);
		trans.position = new Vector3(trans.position.x, trans.position.y, floorHeight);
		floorPosition = new Vector2(trans.position.x, floorHeight);
	}
}
