using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public enum Type {BARRACKS, HOME};
    public List<GameObject> villagerPool;
    public List<GameObject> guardPool;
    public GameObject objects;
    public GameObject entrance {get; private set;}
    public Type type {get; private set;}
    public bool isEnterable {get; private set;}
    public bool isOccupied {get; private set;}
    public int nodeID {get; private set;}
    public float floorHeight {get; private set;}
    public Size size;// {get; private set;}
    private World world;
    private Interior interior;
    private Transform trans;
    private SpriteRenderer rend;
    public void SetNodeID(int nodeID){this.nodeID = nodeID;}
    public void SetType(Type type){this.type=type;}

    public void Init()
    {
        world = GameObject.Find("World").GetComponent<World>();
        interior = World.activeBuilding.GetComponent<Interior>();
        entrance = transform.Find("BuildingEntrance").gameObject;
        trans = GetComponent<Transform>();
        rend = GetComponent<SpriteRenderer>();
        villagerPool = new List<GameObject>();
        guardPool = new List<GameObject>();
        Reset();
    }

    public void Start()
    {
    }

    //called in World.cs when reusing a building pool object after setting its new position
    public void Reset()
    {
        if(objects != null)
        {
            Destroy(objects);
        }
        Debug.Log("Calling Populate in building in node: "+nodeID);
        Populate();
        size = randomSize();
        selectLayout(size);
        setFloorHeight();
        isEnterable = true; //temp
        isOccupied = false; //temp
    }

    //resuses/generates villagers/guards that live in this building
    public void Populate()
    {
        int count = type == Type.HOME ? UnityEngine.Random.Range(1, 5) : UnityEngine.Random.Range(3, 6);
        List<Vector2> points = world.GetValidPoints(nodeID, count);
        List<GameObject> pool = type == Type.HOME ? villagerPool : guardPool;
        for(int i = 0; i < pool.Count; i++)
        {
            if(points.Count == 0)
            {
                for(int j = i; j < pool.Count; j++)
                    pool[j].SetActive(false);
                break;
            }
            Vector2 point = points[0];
            TownspersonClass townie = pool[i].GetComponent<TownspersonClass>();
            townie.trans.position = townie.SetFloorPosition(point);
            townie.Reset();//does not do specific townie reset, just base reset
            pool[i].SetActive(true);
            points.Remove(point);
        }     
        for(int i = 0; i < points.Count; i++)
        {
            GameObject obj = type == Type.HOME ?
                Instantiate(world.villagerPrefabs[UnityEngine.Random.Range(0, world.villagerPrefabs.Count)], trans) :
                Instantiate(world.guardPrefabs[UnityEngine.Random.Range(0, world.guardPrefabs.Count)], trans);
            TownspersonClass townie = obj.GetComponent<TownspersonClass>();
            townie.Init();
            townie.SetNodeID(nodeID);
            townie.SetBuilding(this);
            townie.trans.position = townie.SetFloorPosition(points[i]);
            pool.Add(obj);
        }
    }

    //load into active building
    public void Load(PlayerClass player)
    {
        objects.SetActive(true);
        interior.SetObjects(objects);
        interior.SetBuilding(this);
        interior.SetSize(size);
        interior.SavePlayerPos(player.trans.position);
        player.trans.position = player.SetFloorPosition(interior.spawnPos);
    }

    //remove from active building
    public void Store(PlayerClass player)
    {
        objects.SetActive(false);
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

    //selects a set of objects from the possible blueprints for the size
    private void selectLayout(Size size)
    {
        List<GameObject> blueprints = interior.blueprints[size]; 
        objects = Instantiate(blueprints[UnityEngine.Random.Range(0, blueprints.Count)], interior.transform);
        objects.transform.localPosition = Vector3.zero;
        objects.SetActive(false);
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