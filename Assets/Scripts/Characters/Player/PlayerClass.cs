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
	public int hunger {get; private set;}
	public int gold {get; private set;}
	public PlayerInput input {get; private set;}
	public PlayerActions actions {get; private set;}
	private readonly int BLOODTHIRSTY = 10;
	private readonly int FORCED_HUNGER = 10;
	private readonly float HEALTH_REGEN_DELAY = 6f;
	private float lastDamagedTime = -1f;
	private Collider2D aggroBox;
	public void SetGold(int gold){this.gold=gold;}
	public void SetHunger(int hunger)
	{
		this.hunger = Mathf.Min(hunger, 10);
		this.hunger = Mathf.Max(hunger, 0);
	}
	public void SetLastDamagedTime(float time){this.lastDamagedTime=time;}


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
		hungerHandler();
		healthRegenHandler();
		UpdateAnimator();
	}

	//called by gamestate in masterreset
	public override void Reset()
	{
		SetNodeID(0);
		SetMaxHealth(10);
		SetHealth(maxHealth);
		World.nodes[nodeID].SetActive(true);
		trans.position = SetFloorPosition(World.wnodes[nodeID].playerSpawn.transform.position);
		rend.sprite = human;
		isHuman = true;
		isTrapped = false;
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
			SetHunger(BLOODTHIRSTY);
		}
	}

	private void healthRegenHandler()
	{
		if(lastDamagedTime > 0 && Time.time-lastDamagedTime > HEALTH_REGEN_DELAY)
		{
			lastDamagedTime = -1f;
			StartCoroutine(regenerateHealth(lastDamagedTime));
		}
	}

	//reference time is set to the lastDamagedTime, this way if LDT is reset then regen is interrupted
	private IEnumerator regenerateHealth(float referenceTime)
	{
		Debug.Log("Player is regenerating health!");
		while(lastDamagedTime == referenceTime && health < maxHealth)
		{
			SetHealth(health+1);
			yield return new WaitForSeconds(0.5f);
		}
		if(health == maxHealth)
			Debug.Log("Player is at full health!");
		else
			Debug.Log("Regeneration was interrupted!");
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
