using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public bool isEnterable {get; private set;}
    public int nodeID {get; private set;}
    public Size size {get; private set;}
    public List<GameObject> objects;

    public void SetNodeID(int nodeID){this.nodeID = nodeID;}

    public void Awake()
    {
        objects = new List<GameObject>();
    }

    public void Start()
    {
        size = randomSize();
    }

    public void Update()
    {
    }

    //load into active building
    public void Load(PlayerClass player)
    {
        Interior interior = World.activeBuilding.GetComponent<Interior>();
        interior.SetObjects(objects);
        interior.SetBuilding(this);
        interior.SetColl(size);
        interior.SavePlayerPos(player.trans.position);
        player.trans.position = interior.spawnPos;
    }

    //remove from active building
    public void Store(PlayerClass player)
    {
        Interior interior = World.activeBuilding.GetComponent<Interior>();
        interior.SetObjects(null);
        interior.SetBuilding(null);
        player.trans.position = interior.savedPos;
    }

    //returns a random value from Size enum (in Interior.cs)
    private Size randomSize()
    {
        Size[] values = (Size[])Enum.GetValues(typeof(Size));
        Size size = values[UnityEngine.Random.Range(0, values.Length)];
        return size;
    }
}