using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Size {SMALL, MEDIUM, LARGE};
public class Interior : MonoBehaviour
{
    public List<GameObject> objects {get; private set;}    
    public Building building {get; private set;}
    public Vector2 savedPos {get; private set;}
    public Vector2 spawnPos {get; private set;}
    private GameObject buildingExit;
    private EdgeCollider2D edgeColl;
    private Dictionary<Size, Vector2[]> points = new Dictionary<Size, Vector2[]>
    {
        {Size.SMALL, new Vector2[] {new Vector2(-25f, -2.5f), new Vector2(25f, -2.5f), new Vector2(25f, 50f), new Vector2(-25f, 50f), new Vector2(-25f, -2.5f)}},
        {Size.MEDIUM, new Vector2[] {new Vector2(-30f, -2.5f), new Vector2(30f, -2.5f), new Vector2(30f, 50f), new Vector2(-30f, 50f), new Vector2(-30f, -2.5f)}},
        {Size.LARGE, new Vector2[] {new Vector2(-40f, -2.5f), new Vector2(40f, -2.5f), new Vector2(40f, 50f), new Vector2(-40f, 50f), new Vector2(-40f, -2.5f)}}
    }; 

    public void SetObjects(List<GameObject> objects){this.objects = objects;}
    public void SetBuilding(Building building){this.building = building;}
    public void SavePlayerPos(Vector2 savedPos){this.savedPos = savedPos;} 
    public void SetColl(Size size){edgeColl.points = points[size];}

    public void Awake()
    {
        buildingExit = GameObject.Find("BuildingExit");
        edgeColl = GetComponent<EdgeCollider2D>();
    }

    public void Start()
    {
        spawnPos = buildingExit.transform.position;
    }
}