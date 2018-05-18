using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Action {NONE, TRAVEL};
public class PlayerActions : MonoBehaviour, IHitboxResponder 
{
	private readonly Vector2 travelOffset = new Vector2(0, -12);
	private readonly Vector2 travelSize = new Vector2(5, 2);
	private Hitbox hitbox;

	void Awake()
	{
		hitbox = GetComponent<Hitbox>();
	}

	// Use this for initialization
	void Start() 
	{
	}
	
	// Update is called once per frame
	void Update() 
	{
	}

	public void collisionWith(Collider2D other, Action action)
	{
		switch(action)
		{
			case Action.TRAVEL:
				travel();
				break;
			case Action.NONE:
			default:
				break;
		}
	}

	//triggers hitbox to check for travel colliders
	public void travelCheck()
	{
		Debug.Log("CHECKING FOR TRAVEL");
		hitbox.setResponder(this);
		hitbox.setAction(Action.TRAVEL);
		hitbox.setOffset(travelOffset);
		hitbox.setSize(travelSize);
		hitbox.startCheckingCollision();
		hitbox.checkCollision();
		hitbox.stopCheckingCollision();
	}

	//implement the actual functionality of traveling
	private void travel()
	{
		Debug.Log("Traveling!");
	}
}
