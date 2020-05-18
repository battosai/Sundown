using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class CharacterClass : MonoBehaviour
{
	public readonly float BASE_SPEED = 20f;
    public Sprite alive;
    public Sprite dead;
	[System.NonSerialized] public PathFinding.Node[,] nodeMap;
	public int nodeID {get; private set;}
	public int maxHealth {get; protected set;}
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
	public Hurtbox hurtBox {get; private set;}
	public StateMachine stateMachine {get; protected set;}
    protected float time;
    public bool isAlarmed {get; private set;}
    protected Vector2 alarmPoint;
	protected Animator anim;
	protected Collider2D pushBox;
	public void SetType(CharacterType type){this.type = type;}
	public void SetNodeID(int id){nodeID = id;}
	public void SetAlarmPoint(Vector2 alarmPoint){this.alarmPoint=alarmPoint;}
	public void SetIsLeft(bool isLeft){this.isLeft = isLeft;}
	public void SetIsAlive(bool isAlive){this.isAlive = isAlive;}
    public void SetIsAlarmed(bool isAlarmed){this.isAlarmed=isAlarmed;}
	public void SetSpeed(float speed){this.speed = speed;}
	public void SetHealth(int health)
	{
		this.health = Mathf.Max(health, 0);
		this.health = Mathf.Min(health, this.maxHealth);
	}

	public virtual void Awake()
	{
		gameState = GameObject.Find("GameState").GetComponent<GameState>();
		world = GameObject.Find("World").GetComponent<World>(); 
		trans = GetComponent<Transform>();
		rb = GetComponent<Rigidbody2D>();
		rend = GetComponent<SpriteRenderer>();
		pushBox = GetComponent<Collider2D>();
		hurtBox = GetComponent<Hurtbox>();
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
		setFloorHeight();
	}

	public virtual void Reset()
	{
		speed = BASE_SPEED;
		SetIsAlive(true);
		SetIsAlarmed(false);
		time = Time.time;
	}

	protected void deathPrep()
	{
		SetIsAlive(false);
		rb.velocity = Vector2.zero;
		if(dead == null)
			rend.enabled = false;
		else
			rend.sprite = dead;
		pushBox.enabled = false;
		if(anim != null)
			anim.enabled = false;
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

 	protected virtual IEnumerator dash(Vector2 target, System.Action callback=null)
	{
		float DASH = 40f;
	 	float DASH_TIME = 0.2f;
		float startTime = Time.time;
		while(Time.time-startTime < DASH_TIME)
		{
			rb.velocity = PathFinding.GetVelocity(floorPosition, target, DASH);
			yield return null;
		}
		if(callback != null)
			callback();
	}

	protected IEnumerator wait(float waitTime)
	{	
		float startTime = Time.time;
		while(Time.time-startTime < waitTime)
        {
            rb.velocity = Vector2.zero;
            yield return null;
        }
	}

	protected IEnumerator takePath(Vector2 destination, System.Action callback=null)
	{
		Debug.Log("Taking path!");
		float tolerance = 1f;
		List<Vector2> path = PathFinding.AStarJump(floorPosition, destination, nodeMap, nodeID);
		Debug.DrawLine(new Vector3(destination.x-1f, destination.y, 0f), new Vector3(destination.x+1f, destination.y, 0f), Color.red, 100f);
		for(int i = 0; i < path.Count; i++)
		{
			if(health <= 0)
				break;
			while(true)
			{
				if(health <= 0)
					break;
				if(Vector2.Distance(floorPosition, path[i]) <= tolerance)
					break;
				Debug.DrawLine(new Vector3(path[i].x-1f, path[i].y, 0f), new Vector3(path[i].x+1f, path[i].y, 0f), Color.cyan, 1f);
				rb.velocity = PathFinding.GetVelocity(floorPosition, path[i], speed);
				yield return null;
			}
		}
		Debug.Log("Reached end of path");
		if(callback != null)
			callback();
	}

	public virtual void UpdateAnimator()
	{
		Debug.LogWarning($"{gameObject.name} has no proper UpdateAnimator() method");
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
