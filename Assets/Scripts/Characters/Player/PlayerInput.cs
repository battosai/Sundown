using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: handle player input for all platforms

public class PlayerInput : MonoBehaviour
{
  public static bool isLeftClick;
  public static bool isLeftHold;
  public static bool isRightClick;
  public static bool E, W, A, S, D, Space;
	private readonly float DEFAULT_MOUSE_Z = -9f;
  private readonly float CLICK_TOLERANCE = 0.2f;
  private readonly float UNSET_TIME = -1f;
	private readonly float FORCE_SHAPESHIFT= 3f;
  private float downTime;
  private float holdTime;
	private GameObject mouse;
	private Transform mouseTrans;
	private SpriteRenderer mouseRend;
	private Camera cam;
  private PlayerClass player;
  public Vector2 GetMousePos(){return mouseTrans.position;}

	void Awake()
	{
		mouse = GameObject.Find("Mouse");
		mouseTrans = mouse.GetComponent<Transform>();
		mouseRend = mouse.GetComponent<SpriteRenderer>();
		cam = GameObject.Find("MainCamera").GetComponent<Camera>();
    player = GameObject.Find("Player").GetComponent<PlayerClass>();
  }

	// Use this for initialization
	void Start()
	{
		downTime = UNSET_TIME;
    holdTime = UNSET_TIME;
	}

	// Update is called once per frame
	public void Update()
	{
		trackMouse();
    getMouseInput();
    getKeyboardInput();
    useInput();
	}

  private void useInput()
  {
    player.rb.velocity = calculateVelocity();
    if(player.rb.velocity.x == 0f)
    {
      if(mouseTrans.position.x > player.trans.position.x)
        player.SetIsLeft(false);
      else if(mouseTrans.position.x < player.trans.position.x)
        player.SetIsLeft(true);
    }
    else
      player.SetIsLeft(player.rb.velocity.x < 0);
    player.rend.flipX = !player.isLeft;
    if(isRightClick)
      player.actions.AttackCheck();
    if(E)
      player.actions.InteractCheck();
    if(isLeftHold && holdTime > FORCE_SHAPESHIFT && player.isHuman)
    {
      downTime = UNSET_TIME;
      player.Shapeshift(true);
      player.actions.ShapeshiftCheck();
    }
  }

  private Vector2 calculateVelocity()
  {
    float x = 0;
    float y = 0;
    if(W)
      y = player.speed;
    else if(S)
      y = -player.speed;
    if(D)
      x = player.speed;
    else if(A)
      x = -player.speed;
    return new Vector2(x, y);
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
    {
      downTime = Time.time;
    }
    isLeftClick = false;
    if(Input.GetMouseButtonUp(0))
    {
      holdTime = Time.time - downTime;
      isLeftClick = holdTime < CLICK_TOLERANCE;
			downTime = UNSET_TIME;
    }
		isLeftHold = false;
		if(downTime != UNSET_TIME)
		{
			holdTime = Time.time - downTime;
			if(holdTime > CLICK_TOLERANCE)
				isLeftHold = true;
		}
    isRightClick = Input.GetMouseButtonDown(1);
  }

  private void getKeyboardInput()
  {
    E = Input.GetKeyDown(KeyCode.E);
    W = Input.GetKey(KeyCode.W);
    A = Input.GetKey(KeyCode.A);
    S = Input.GetKey(KeyCode.S);
    D = Input.GetKey(KeyCode.D);
    Space = Input.GetKeyDown(KeyCode.Space);
    if(W && S)
    {
      W = false;
      S = false;
    }
    if(A && D)
    {
      A = false;
      D = false;
    }
  }
}
