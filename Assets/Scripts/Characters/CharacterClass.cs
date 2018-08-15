using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType {PLAYER, HERO, WILDLIFE, TOWNSPERSON};
public class CharacterClass : MonoBehaviour
{
	public readonly float BASE_SPEED = 20f;
	public int nodeID {get; private set;}
	public int health {get; private set;}
	public float speed {get; private set;}
	public float floorHeight {get; private set;}
	public bool isLeft {get; private set;}
	public bool isAlive {get; private set;}
	public Vector2 floorPosition {get; private set;}
	public CharacterType type {get; private set;}
	public GameState gameState {get; private set;}
	public World world {get; private set;}
	public Transform trans {get; private set;}
	public Rigidbody2D rb {get; private set;}
	public SpriteRenderer rend {get; private set;}
	protected int maxHealth;
    protected float time;
    protected bool isAlarmed;
	protected PathFinding.Node[,] nodeMap;
	protected Animator anim;
	protected Collider2D pushBox;
	public void SetType(CharacterType type){this.type = type;}
	public void SetNodeID(int id){nodeID = id;}
	public void SetIsLeft(bool isLeft){this.isLeft = isLeft;}
	public void SetIsAlive(bool isAlive){this.isAlive = isAlive;}
    public void SetIsAlarmed(bool isAlarmed){this.isAlarmed=isAlarmed;}
    public void SetMaxHealth(int maxHealth){this.maxHealth=maxHealth;}
	public void SetHealth(int health){this.health = health;}
	public void SetSpeed(float speed){this.speed = speed;}

	public virtual void Awake()
	{
		gameState = GameObject.Find("GameState").GetComponent<GameState>();
		world = GameObject.Find("World").GetComponent<World>(); 
		trans = GetComponent<Transform>();
		rb = GetComponent<Rigidbody2D>();
		rend = GetComponent<SpriteRenderer>();
		pushBox = GetComponent<Collider2D>();
		anim = GetComponent<Animator>();
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
		SetIsAlive(true);
		SetIsAlarmed(false);
		time = Time.time;
	}

  	protected void idleWalk()
    {
        int action = Random.Range(0, 5);
        if(action == 0)
        {
            float horizontal = Random.Range(-speed, speed);
            float vertical = Random.Range(-speed, speed);
            rb.velocity = new Vector2(horizontal, vertical);
        }
        else
            rb.velocity = Vector2.zero;
    } 

	protected IEnumerator takePath(Vector2 destination)
	{
		Debug.Log("Taking path!");
		float tolerance = 1f;
		List<Vector2> path = PathFinding.AStarJump(floorPosition, destination, nodeMap, nodeID);
		Debug.DrawLine(new Vector3(destination.x-1f, destination.y, 0f), new Vector3(destination.x+1f, destination.y, 0f), Color.cyan, 100f);
		for(int i = 0; i < path.Count; i++)
		{
			while(true)
			{
				if(Vector2.Distance(floorPosition, path[i]) <= tolerance)
					break;
				Debug.DrawLine(new Vector3(path[i].x-1f, path[i].y, 0f), new Vector3(path[i].x+1f, path[i].y, 0f), Color.cyan, 1f);
				rb.velocity = PathFinding.GetVelocity(floorPosition, path[i], speed);
				yield return null;
			}
		}
		Debug.Log("Reached end of path");
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
