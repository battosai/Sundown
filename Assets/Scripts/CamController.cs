using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: controls how the camera behaves

public class CamController : MonoBehaviour
{
	private Transform trans;
	private PlayerClass player;

	void Awake()
	{
		player = GameObject.Find("Player").GetComponent<PlayerClass>();
		trans = GetComponent<Transform>();
	}

	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{
		trans.position = new Vector3(player.trans.position.x, player.trans.position.y, trans.position.z);
	}
}
