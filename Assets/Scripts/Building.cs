using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public bool isEnterable {get; private set;}
    public int nodeID {get; private set;}
    public float floorHeight {get; private set;}
    public Size size {get; private set;}
    public List<GameObject> objects;
    private Transform trans;
    private SpriteRenderer rend;

    public void SetNodeID(int nodeID){this.nodeID = nodeID;}

    public void Init()
    {
        objects = new List<GameObject>();
        trans = GetComponent<Transform>();
        rend = GetComponent<SpriteRenderer>();
    }

    public void Start()
    {
        size = randomSize();
        setFloorHeight();
    }

    //called in World.cs when reusing a building pool object after setting its new position
    public void  Reset()
    {
        size = randomSize();
        setFloorHeight();
    }

    //load into active building
    public void Load(PlayerClass player)
    {
        Interior interior = World.activeBuilding.GetComponent<Interior>();
        interior.SetObjects(objects);
        interior.SetBuilding(this);
        interior.SetColl(size);
        interior.SavePlayerPos(player.trans.position);
        player.trans.position = player.SetFloorPosition(interior.spawnPos);
    }

    //remove from active building
    public void Store(PlayerClass player)
    {
        Interior interior = World.activeBuilding.GetComponent<Interior>();
        interior.SetObjects(null);
        interior.SetBuilding(null);
        player.trans.position = interior.savedPos;
    }
    
    //sets position so that floor position is at target
	public Vector2 SetFloorPosition(Vector2 target)
	{
		float yOffset = rend.bounds.size.y/2;
		return new Vector2(target.x, target.y+yOffset);
	}

    //returns a random value from Size enum (in Interior.cs)
    private Size randomSize()
    {
        Size[] values = (Size[])Enum.GetValues(typeof(Size));
        Size size = values[UnityEngine.Random.Range(0, values.Length)];
        return size;
    }

    private void setFloorHeight()
    {
		floorHeight = trans.position.y-(rend.bounds.size.y/2);
		trans.position = new Vector3(trans.position.x, trans.position.y, floorHeight);
    }
}