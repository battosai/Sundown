using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColliderGenerator : MonoBehaviour
{
    private MapGenerator mapGen;

    public void Awake()
    {
        mapGen = GetComponent<MapGenerator>();
    }

    public void GenerateCollider(GameObject node)
    {
        MeshFilter meshFilter = node.GetComponent<MeshFilter>();
        if(meshFilter == null)
        {
            Debug.Log("[Error] No Mesh Filter!");
            return;
        }
        List<Edge> edges = createEdges(meshFilter.mesh.vertices, meshFilter.mesh.triangles);
        List<List<Vector2>> rooms = parseEdges(edges);
        useColliderPool(rooms, node);
    }

    private void useColliderPool(List<List<Vector2>> rooms, GameObject node)
    {
        List<GameObject> collPool = node.GetComponent<WorldNode>().collPool;
        for(int i = 0; i < collPool.Count; i++)
        {
            if(rooms.Count == 0)
            {
                for(int j = i; j < collPool.Count; j++)
                {
                    GameObject extraColl = collPool[j];
                    extraColl.SetActive(false);        
                }
                break;
            }
            GameObject coll = collPool[i];
            List<Vector2> room = rooms[0];
            coll.SetActive(true);
            coll.GetComponent<EdgeCollider2D>().points = room.ToArray();
            rooms.Remove(room); 
        }
        if(rooms.Count > 0)
            createColliders(rooms, node); 
    }

    //creates edge colliders for each room attached to a child object of the respective node
    //used if node has no unused colliders in pool, add created ones to pool
    private void createColliders(List<List<Vector2>> rooms, GameObject node)
    {
        WorldNode wnode = node.GetComponent<WorldNode>();
        foreach(List<Vector2> room in rooms)
        {
            GameObject child = new GameObject("EdgeCollider");
            child.layer = LayerMask.NameToLayer("PushBox");
            child.AddComponent<Rigidbody2D>();
            Rigidbody2D rb = child.GetComponent<Rigidbody2D>();
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.gravityScale = 0f;
            rb.angularDrag = 0f;
            child.AddComponent<EdgeCollider2D>();
            child.GetComponent<EdgeCollider2D>().points = room.ToArray();
            child.transform.parent = node.transform;
            child.transform.localPosition = Vector2.zero;
            wnode.AddCollPool(child);
        }
    }

    //groups points from edges into their respective rooms and in proper order
    private List<List<Vector2>> parseEdges(List<Edge> edges)
    {
        List<List<Vector2>> rooms = new List<List<Vector2>>();
        List<Edge> used = new List<Edge>();
        while(edges.Count > 0)
        {
            bool connected = false;
            List<Vector2> room = new List<Vector2>();
            Vector2[] endPoints = new Vector2[2];
            while(!connected) 
            {
                for(int i = 0; i < edges.Count; i++)
                {
                    Edge edge = edges[i];
                    //first edge that initializes both endpoints
                    if(room.Count == 0)
                    {
                        room.Add(edge.b);
                        room.Add(edge.a);
                        endPoints[0] = edge.a;
                        endPoints[1] = edge.b;
                        edges.Remove(edge);
                    }
                    //last edge that unites both endpoints
                    else if((edge.a == endPoints[0] && edge.b == endPoints[1]) || (edge.b == endPoints[0] && edge.a == endPoints[1]))
                    {
                        room.Add(edge.a);
                        edges.Remove(edge);
                        connected = true;
                        break;
                    }
                    //edge that only connects to endpoint[0] (to preserve the order)
                    else if(edge.a == endPoints[0] || edge.b == endPoints[0])
                    {
                        room.Add(edge.a);
                        endPoints[0] = edge.a == endPoints[0] ? edge.b : edge.a;
                        edges.Remove(edge);
                    }
                }
            }
            rooms.Add(room);
        }
        return rooms;
    }

    //acquires list of edges based on an edge only belonging to 1 mesh triangle bc its the outter side
    private List<Edge> createEdges(Vector3[] vertices, int[] triangles)
    {
        List<Edge> edges = new List<Edge>();
        //filter out edges that appear in multiple triangles with a dictionary
        Dictionary<string, KeyValuePair<int, int>> unique = new Dictionary<string, KeyValuePair<int, int>>();
        for(int i = 0; i < triangles.Length; i += 3)
        {
            for(int e = 0; e < 3; e++)
            {
                int vertA = triangles[i+e];
                int vertB = i+e+1 > i+2 ? triangles[i] : triangles[i+e+1];
                string edge = Mathf.Min(vertA, vertB) + ":" + Mathf.Max(vertA, vertB);
                //edges that are already in the dictionary belong to 2 mesh triangles and thus can be removed
                if(unique.ContainsKey(edge))
                    unique.Remove(edge);
                else
                    unique.Add(edge, new KeyValuePair<int, int>(vertA, vertB));
            }
        }
        //change dictionary values (keyvaluepair) into edges for list
        foreach(KeyValuePair<int, int> edge in unique.Values)
        {
            Vector2 key = (Vector2) vertices[edge.Key];
            Vector2 value = (Vector2) vertices[edge.Value];
            edges.Add(new Edge(key, value));
        }
        return edges;
    }

    private class Edge
    {
        public Vector2 a, b;
        public Edge(Vector2 a, Vector2 b)
        {
            this.a = a;
            this.b = b;
        }
    }
}
