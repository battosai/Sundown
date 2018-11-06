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
	public bool foundMap {get; private set;}
	public int strength {get; private set;}
	public int hunger {get; private set;}
	public int gold {get; private set;}
	public new float time {get; private set;}
	public PlayerInput input {get; private set;}
	public PlayerActions actions {get; private set;}
	private readonly int BLOODTHIRSTY = 10;
	private readonly int FORCED_HUNGER = 10;
	private Collider2D aggroBox;
	public void SetIsTrapped(bool isTrapped){this.isTrapped=isTrapped;}
	public void SetFoundMap(bool foundMap){this.foundMap=foundMap;}
	public void SetGold(int gold){this.gold=gold;}
	public void SetHunger(int hunger)
	{
		if(hunger >= 0)
			this.hunger = Mathf.Min(hunger, 10);
		else
			this.hunger = Mathf.Max(hunger, 0);
	}

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
		foundMap = false;
		if(health <= 0)
		{
			Application.Quit();
			// UnityEditor.EditorApplication.isPlaying = false;
		}
		setFloorHeight();
		hungerHandler();
		UpdateAnimator();
	}

	//called by gamestate in masterreset
	public override void Reset()
	{
		SetNodeID(0);
		SetMaxHealth(10);
		SetHealth(maxHealth);
		SetIsTrapped(false);
		World.nodes[nodeID].SetActive(true);
		trans.position = SetFloorPosition(World.wnodes[nodeID].playerSpawn.transform.position);
		rend.sprite = human;
		isHuman = true;
		strength = 1;
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

	public void BecomeTrapped()
	{
		SetIsTrapped(true);
		time = Time.time;
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
			SetHunger(BLOODTHIRSTY);
		}
	}

	//handles player form with respect to hunger levels
	private void hungerHandler()
	{
		if(hunger >= BLOODTHIRSTY && isHuman)
		{
			Shapeshift();
			actions.ShapeshiftCheck();
		}
		else if(hunger == 0 && !isHuman)
		{
			Shapeshift();
		}
	}
}
