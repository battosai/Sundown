using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Size {SMALL, MEDIUM, LARGE};
public class Interior : MonoBehaviour
{
    public Vector3[] outter;
    public List<GameObject> objects {get; private set;}    
    public Building building {get; private set;}
    public Vector2 savedPos {get; private set;}
    public Vector2 spawnPos {get; private set;}
    private static readonly float SMALL = 25f;
    private static readonly float MEDIUM = 30f;
    private static readonly float LARGE = 40f;
    private GameObject buildingExit;
    private MeshFilter meshFilter;
    private EdgeCollider2D edgeColl;
    private Dictionary<Size, Mesh> meshPool;
    private Dictionary<Size, Vector2[]> points = new Dictionary<Size, Vector2[]>
    {
        {Size.SMALL, new Vector2[] {new Vector2(-SMALL, -SMALL), new Vector2(SMALL, -SMALL), new Vector2(SMALL, SMALL), new Vector2(-SMALL, SMALL), new Vector2(-SMALL, -SMALL)}},
        {Size.MEDIUM, new Vector2[] {new Vector2(-MEDIUM, -MEDIUM), new Vector2(MEDIUM, -MEDIUM), new Vector2(MEDIUM, MEDIUM), new Vector2(-MEDIUM, MEDIUM), new Vector2(-MEDIUM, -MEDIUM)}},
        {Size.LARGE, new Vector2[] {new Vector2(-LARGE, -LARGE), new Vector2(LARGE, -LARGE), new Vector2(LARGE, LARGE), new Vector2(-LARGE, LARGE), new Vector2(-LARGE, -LARGE)}}
    }; 

    public void SetObjects(List<GameObject> objects){this.objects = objects;}
    public void SetBuilding(Building building){this.building = building;}
    public void SavePlayerPos(Vector2 savedPos){this.savedPos = savedPos;} 
    public void SetSize(Size size)
    {
        edgeColl.points = points[size];
        meshFilter.mesh = getMesh(size); 
        float dimension = -Mathf.Abs(points[size][0][0]); //exit sprite is centered in image, so no need to do sprite dimensions
        buildingExit.transform.position = new Vector2(0f, dimension);
        spawnPos = buildingExit.transform.position;
    }

    public void Awake()
    {
        buildingExit = GameObject.Find("BuildingExit");
        meshFilter = GetComponent<MeshFilter>();
        edgeColl = GetComponent<EdgeCollider2D>();
    }

    public void Start()
    {
        meshPool = new Dictionary<Size, Mesh>();
    }

    //checks pool for mesh, otherwise add a new one to the pool
    private Mesh getMesh(Size size)
    {
        if(meshPool.ContainsKey(size))
            return meshPool[size];
        Mesh mesh = createMesh(size);
        meshPool[size] = mesh;
        return mesh;
    }

    //create new mesh for designated size
    private Mesh createMesh(Size size)
    {
        Mesh mesh = new Mesh();
        Vector2[] inner = points[size];
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        foreach(Vector2 pt in inner)
            vertices.Add(new Vector3(pt.x, pt.y));
        vertices.AddRange(outter);
        switch(size)
        {
            case Size.SMALL:
                goto case Size.LARGE;
            case Size.MEDIUM:
                goto case Size.LARGE;
            case Size.LARGE:
                for(int i = 0; i < inner.Length-1; i++)
                {
                   triangles.Add(i);
                   triangles.Add(inner.Length+i+1);
                   triangles.Add(inner.Length+i);
                   triangles.Add(i);
                   triangles.Add(i+1);
                   triangles.Add(inner.Length+i+1);
                }
                break;
            default:
                Debug.Log("[Error] Unrecognized Size Enumeration Value!");
                break;
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }
}