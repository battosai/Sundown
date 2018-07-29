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
	public int food {get; private set;}
	public int hunger {get; private set;}
	public int gold {get; private set;}
	public PlayerInput input {get; private set;}
	public PlayerActions actions {get; private set;}
	public Collider2D pushBox {get; private set;}
	private readonly int succumbedToHungerPenalty = 10;
	private Animator anim;
	public void SetFood(int food){this.food=food;}
	public void SetGold(int gold){this.gold=gold;}

	public override void Awake()
	{
		base.Awake();
		input = GetComponent<PlayerInput>();
		actions = GetComponent<PlayerActions>();
		pushBox = GetComponent<Collider2D>();
		anim = GetComponent<Animator>();
	}

	public void Start()
	{
		SetType(CharacterType.PLAYER);
		setFloorHeight();
	}

	// Update is called once per frame
	void Update()
	{
		setFloorHeight();
		hungerHandler();
		UpdateAnimator();
	}

	//called by gamestate in masterreset
	public override void Reset()
	{
		base.Reset();
		SetNodeID(0);
		World.nodes[nodeID].SetActive(true);
		trans.position = SetFloorPosition(World.wnodes[nodeID].playerSpawn.transform.position);
		rend.sprite = human;
		isHuman = true;
		strength = 5;
		food = 5;
		hunger = 0;
		gold = 0;
		UpdateAnimator();
	}

	public override void UpdateAnimator()
	{
		anim.SetBool("isHuman", isHuman);
		anim.SetFloat("speed", Mathf.Abs(rb.velocity.x)+Mathf.Abs(rb.velocity.y));
	}

	//called whenever player goes to next node or maybe by will
	public void Shapeshift()
	{
		isHuman = !isHuman;
		if(isHuman)
		{
			rend.sprite = human;
		}
		else
		{
			rend.sprite = werewolf; //will be half human eventually
			SetFood(food+succumbedToHungerPenalty);
		}
	}

	//handles player form with respect to hunger/food levels
	private void hungerHandler()
	{
		if(hunger > food && isHuman)
		{
			Shapeshift();
		}
		else if(hunger <= food && !isHuman)
		{
			Shapeshift();
		}
	}

}
