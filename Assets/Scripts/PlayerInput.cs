using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	private readonly float DEFAULT_MOUSE_Z = -9f;
  private readonly float CLICK_TOLERANCE = 0.25f;
  private readonly float UNSET_TIME = -1f;

  private bool isClick;
  private bool isHold;
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
    if(Input.GetMouseButtonUp(0))
      downTime = UNSET_TIME;
    if(downTime != UNSET_TIME)
    {
      float duration = Time.time - downTime;
      isHold = duration > CLICK_TOLERANCE;
      isClick = !isHold;
    }
  }
}
