using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: controls player status
//set all objects with sprites to use floorHeight to decide rendering order

public class PlayerClass : CharacterClass
{
	public Sprite human, halfHuman, werewolf;
	public bool isHuman {get; private set;}
	public int strength {get; private set;}
	public int hunger {get; private set;}
	public int gold {get; private set;}
	public PlayerInput input {get; private set;}
	public PlayerActions actions {get; private set;}
	private readonly int BLOODTHIRSTY = 10;
	private readonly int FORCED_HUNGER = 40;
	private readonly int TOO_HUNGRY = 20;
	private Collider2D aggroBox;
	public void SetHunger(int hunger){this.hunger=hunger;}
	public void SetGold(int gold){this.gold=gold;}

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
	}

	// Update is called once per frame
	public override void Update()
	{
		if(health <= 0)
		{
			//Application.Quit();
			UnityEditor.EditorApplication.isPlaying = false;
		}
		setFloorHeight();
		hungerHandler();
		UpdateAnimator();
	}

	//called by gamestate in masterreset
	public override void Reset()
	{
		SetNodeID(0);
		SetMaxHealth(20);
		SetHealth(maxHealth);
		World.nodes[nodeID].SetActive(true);
		trans.position = SetFloorPosition(World.wnodes[nodeID].playerSpawn.transform.position);
		rend.sprite = human;
		isHuman = true;
		strength = 5;
		hunger = 0;
		gold = 0;
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

	//called whenever player goes to next node or maybe by will
	public void Shapeshift(bool isForced=false)
	{
		isHuman = !isHuman;
		aggroBox.enabled = !isHuman;
		if(isHuman)
		{
			rend.sprite = human;
		}
		else
		{
			rend.sprite = werewolf; //will be half human eventually
			if(isForced)
				SetHunger(hunger+FORCED_HUNGER);
			else
				SetHunger(hunger+BLOODTHIRSTY);
		}
	}

	//handles player form with respect to hunger levels
	private void hungerHandler()
	{
		if(hunger > TOO_HUNGRY && isHuman)
		{
			Shapeshift();
			actions.ShapeshiftCheck();
		}
		else if(hunger <= TOO_HUNGRY && !isHuman)
		{
			Shapeshift();
		}
	}
}
