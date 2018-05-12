using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: handle player inputs for all platforms

public class PlayerInput : MonoBehaviour
{
  public static bool isClick;
  public static bool isHold;

	private readonly float DEFAULT_MOUSE_Z = -9f;
  private readonly float CLICK_TOLERANCE = 0.2f;
  private readonly float UNSET_TIME = -1f;

  private float downTime;
	private GameObject mouse;
	private Transform mouseTrans;
	private SpriteRenderer mouseRend;
	private Camera cam;

	void Awake()
	{
		mouse = GameObject.Find("Mouse");
		mouseTrans = mouse.GetComponent<Transform>();
		mouseRend = mouse.GetComponent<SpriteRenderer>();
		cam = GameObject.Find("MainCamera").GetComponent<Camera>();
	}

	// Use this for initialization
	void Start()
	{
		downTime = UNSET_TIME;
	}

	// Update is called once per frame
	void Update()
	{
		trackMouse();
    getMouseInput();
	}

	private void trackMouse()
	{
		Vector2 mousePos = Input.mousePosition;
		mousePos = cam.ScreenToWorldPoint(mousePos);
		mouseTrans.position = new Vector3(mousePos.x, mousePos.y, DEFAULT_MOUSE_Z);
	}

  //differentiates between mouse click and hold
  private void getMouseInput()
  {
    if(Input.GetMouseButtonDown(0))
      downTime = Time.time;
    isClick = false;
    if(Input.GetMouseButtonUp(0))
    {
      float duration = Time.time - downTime;
      isClick = duration < CLICK_TOLERANCE;
			downTime = UNSET_TIME;
    }
		isHold = false;
		if(downTime != UNSET_TIME)
		{
			float duration = Time.time - downTime;
			if(duration > CLICK_TOLERANCE)
				isHold = true;
		}
		// if(isHold)
		// 	Debug.Log("Hold");
		// if(isClick)
		// 	Debug.Log("Click");
  }
}
