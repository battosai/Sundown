using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interior : MonoBehaviour
{
    public List<GameObject> objects {get; private set;}    
    public Building building {get; private set;}
    public Vector2 savedPos {get; private set;}
    public Vector2 spawnPos {get; private set;}
    private GameObject buildingExit;

    public void SetObjects(List<GameObject> objects){this.objects = objects;}
    public void SetBuilding(Building building){this.building = building;}
    public void SavePlayerPos(Vector2 savedPos){this.savedPos = savedPos;} 

    public void Awake()
    {
        buildingExit = GameObject.Find("BuildingExit");
    }

    public void Start()
    {
        spawnPos = buildingExit.transform.position;
    }
}