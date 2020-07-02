using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public abstract class CharacterClass : MonoBehaviour
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
	public GameState gameState {get; private set;}
	public World world {get; private set;}
	public Transform trans {get; private set;}
	public Rigidbody2D rb {get; private set;}
	public SpriteRenderer rend {get; private set;}
	public Hurtbox hurtBox {get; private set;}
	public StateMachine stateMachine {get; protected set;}
	public Collider2D pushBox {get; private set;}
    protected float time;
    public bool isAlarmed {get; private set;}
    // protected Vector2 alarmPoint;
	protected Animator anim;
	public void SetNodeID(int id){nodeID = id;}
	// public void SetAlarmPoint(Vector2 alarmPoint){this.alarmPoint=alarmPoint;}
	public void SetIsLeft(bool isLeft){this.isLeft = isLeft;}
	public void SetIsAlive(bool isAlive){this.isAlive = isAlive;}
    public void SetIsAlarmed(bool isAlarmed){this.isAlarmed=isAlarmed;}
	public void SetSpeed(float speed){this.speed = speed;}
	public void SetHealth(int health)
	{
		this.health = Mathf.Max(health, 0);
		this.health = Mathf.Min(health, this.maxHealth);
	}

	protected abstract void UpdateAnimator();

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
