using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: controls player status
//set all objects with sprites to use floorHeight to decide rendering order

public class PlayerClass : CharacterClass
{
	public Sprite human, halfHuman, werewolf;
	public bool isHuman {get; private set;}
	public bool isTrapped {get; private set;}
	public int strength {get; private set;}
	public PlayerInput input {get; private set;}
	public PlayerActions actions {get; private set;}
	private Collider2D aggroBox;

	public override void Awake()
	{
		base.Awake();
		aggroBox = GameObject.Find("WerewolfAggroBox").GetComponent<Collider2D>();
		input = GetComponent<PlayerInput>();
		actions = GetComponent<PlayerActions>();
	}

	public void Start()
	{
		aggroBox.enabled = false;
		SetType(CharacterClass.Type.PLAYER);
		setFloorHeight();
		Trap.OnTrapped += Trapped;
	}

	// Update is called once per frame
	public override void Update()
	{
		if(health <= 0)
		{
			#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
			#else
				Application.Quit();
			#endif
		}
		setFloorHeight();
		UpdateAnimator();
	}

	//called by gamestate in masterreset
	public override void Reset()
	{
		SetNodeID(0);
		maxHealth = 10;
		SetHealth(maxHealth);
		World.nodes[nodeID].SetActive(true);
		trans.position = SetFloorPosition(World.wnodes[nodeID].playerSpawn.transform.position);
		rend.sprite = human;
		isHuman = true;
		isTrapped = false;
		strength = 1;
		UpdateAnimator();
		actions.Reset();
		base.Reset();
	}

	public override void UpdateAnimator()
	{
		if(actions.isAttacking)
			anim.SetTrigger("isAttacking");
		if(actions.isAttackOnCooldown)
			anim.SetTrigger("isAttackOnCooldown");
		anim.SetBool("isHuman", isHuman);
		anim.SetFloat("speed", Mathf.Abs(rb.velocity.x)+Mathf.Abs(rb.velocity.y));
		anim.SetInteger("attackCount", input.attackCount);
	}

	//subscribed to Trap.OnTrapped event
	private void Trapped()
	{
		isTrapped = true;	
		rb.velocity = Vector2.zero;
		StartCoroutine(TrappedDuration());
	}

	//begins when player steps in trap;
	private IEnumerator TrappedDuration()
	{
		float time = 0f;
		while(time <= Trap.TRAP_TIME)
		{
			time += Time.deltaTime;
			yield return null;
		}
		isTrapped = false;
	}

	//called whenever player goes to next node or maybe by will
	public void Shapeshift()
	{
		isHuman = !isHuman;
		aggroBox.enabled = !isHuman;
		if(isHuman)
		{
			rend.sprite = human;
		}
		else
		{
			rend.sprite = werewolf;
		}
	}
}
