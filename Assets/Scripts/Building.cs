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

    public void Load()
    {
        Interior interior = World.activeBuilding.GetComponent<Interior>();
        interior.SetObjects(objects);
       //load into activeBuilding 
    }

    public void Store()
    {
        Interior interior = World.activeBuilding.GetComponent<Interior>();
        interior.SetObjects(null);
        //take from activeBuilding
    }
}