using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColliderGenerator : MonoBehaviour
{
    private List<List<Vector2>> points;
    private MapGenerator mapGen;

    public void Awake()
    {
        mapGen = GetComponent<MapGenerator>();
    }

    public void GenerateCollider(GameObject node)
    {
        points = new List<List<Vector2>>();
        MeshFilter meshFilter = node.GetComponent<MeshFilter>();
        if(meshFilter == null)
        {
            Debug.Log("[Error] No Mesh Filter!");
            return;
        }
        points = mapGen.edgePoints;
        foreach(List<Vector2> roomPoints in points)
        {
            GameObject child = new GameObject("EdgeCollider");
            child.AddComponent<Rigidbody2D>();
            Rigidbody2D rb = child.GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.angularDrag = 0f;
            child.AddComponent<EdgeCollider2D>();
            child.GetComponent<EdgeCollider2D>().points = roomPoints.ToArray();
            child.transform.parent = node.transform;
        }
        //THE POINTS MIGHT NEED TO BE IN ORDER!
    }

    //Does the same thing as mapgen.edgePoints attribute, but this doesn't differentiate between rooms
    //NOT FINISHED, DOESNT RETURN VECTOR2s
    // private List<Vector2> getEdgePoints(Vector3[] vertices, int[] triangles)
    // {
    //     //use the triangles to find which points are connected
    //     List<Vector2> edgePoints = new List<Vector2>();
    //     Dictionary<string, KeyValuePair<int, int>> unique = new Dictionary<string, KeyValuePair<int, int>>();
    //     for(int i = 0; i < triangles.Length; i += 3)
    //     {
    //         for(int e = 0; e < 3; e++)
    //         {
    //             int vertA = triangles[i+e];
    //             int vertB = i+e+1 > i+2 ? triangles[i] : triangles[i+e+1];
    //             string edge = Mathf.Min(vertA, vertB) + ":" + Mathf.Max(vertA, vertB);
    //             if(unique.ContainsKey(edge))
    //                 unique.Remove(edge);
    //             else
    //                 unique.Add(edge, new KeyValuePair<int, int>(vertA, vertB));
    //         }
    //     }
    //     Dictionary<int, int> edgeVerts = new Dictionary<int, int>();
    //     foreach(KeyValuePair<int, int> edge in unique.Values)
    //     {
    //         if(!edgeVerts.ContainsKey(edge.Key))
    //             edgeVerts.Add(edge.Key, edge.Value);
    //         else
    //             Debug.Log("hello");
    //     }
    //     Debug.Log(edgeVerts.Count);
    //     return edgePoints;
    // }

    // private void removeDuplicateEdges()
    // {
    //     List<Edge> edgesToBeRemoved = new List<Edge>();
    //     foreach(Edge a in edges)
    //     {
    //         foreach(Edge b in edges)
    //         {
    //             if(a == b)
    //                 continue;
    //             if((a.a == b.a && a.b == b.b) || (a.b == b.a && a.a == b.b))
    //                 edgesToBeRemoved.Add(a);
    //         }
    //     }
    //     foreach(Edge removable in edgesToBeRemoved)
    //         edges.Remove(removable);
    // }

    // private class Edge
    // {
    //     public Vector2 a, b;
    //     public Edge(Vector2 a, Vector2 b)
    //     {
    //         this.a = a;
    //         this.b = b;
    //     }
    // }
}