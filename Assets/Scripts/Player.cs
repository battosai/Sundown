using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: handles player controls and input

public class Player : MonoBehaviour
{
  //public, only writeable in-class
	public int nodeID {get; private set;}
	public boolean isHuman {get; private set;}
	private Transform trans;
	private SpriteRenderer rend;

	//temporary, only used in editor for user input (will be touch)
	private readonly float defaultMouseZ = -10f;
	private GameObject mouse;
	private Transform mouseTrans;
	private SpriteRenderer mouseRend;
	private Camera cam;

	void Awake()
	{
		trans = GetComponent<Transform>();
		rend = GetComponent<SpriteRenderer>();
		//temp
		mouse = GameObject.Find("Mouse");
		mouseTrans = mouse.GetComponent<Transform>();
		mouseRend = mouse.GetComponent<SpriteRenderer>();
		cam = GameObject.Find("MainCamera");
	}

	// Use this for initialization
	void Start()
	{
		reset();
	}

	// Update is called once per frame
	void Update()
	{
		trackMouse();
	}

	//will also be used when replaying game
	//place all initializations in here
	public void reset()
	{
		isHuman = true;
	}

	//temp
	private void trackMouse()
	{
		Vector2 mousePos = Input.mousePosition;
		mousePos = cam.ScreenToWorldPoint(mousePos);
		mouseTrans.position = new Vector3(mousePos.x, mousePos.y, defaultMouseZ);
	}
}
