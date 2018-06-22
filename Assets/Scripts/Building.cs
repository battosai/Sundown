using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public bool isEnterable {get; private set;}
    public int nodeID {get; private set;}
    public List<GameObject> objects;

    public void SetNodeID(int nodeID){this.nodeID = nodeID;}

    public void Awake()
    {
        objects = new List<GameObject>();
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
}