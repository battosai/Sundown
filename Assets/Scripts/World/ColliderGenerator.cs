using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColliderGenerator : MonoBehaviour
{
    private List<Edge> edges;
    private List<Vector2> points;

    public void GenerateCollider(GameObject node)
    {
        points = new List<Vector2>();
        edges = new List<Edge>();
        MeshFilter meshFilter = node.GetComponent<MeshFilter>();
        if(meshFilter == null)
        {
            Debug.Log("[Error] No Mesh Filter!");
            return;
        }
        Vector3[] vertices = meshFilter.mesh.vertices;
        int[] triangles = meshFilter.mesh.triangles;
        //get only outter edges from mesh
        points = getEdgePoints(vertices, triangles);
    }

    private List<Vector2> getEdgePoints(Vector3[] vertices, int[] triangles)
    {
        List<Vector2> points = new List<Vector2>();
        return points;
    }

    private void removeDuplicateEdges()
    {
        List<Edge> edgesToBeRemoved = new List<Edge>();
        foreach(Edge a in edges)
        {
            foreach(Edge b in edges)
            {
                if(a == b)
                    continue;
                if((a.a == b.a && a.b == b.b) || (a.b == b.a && a.a == b.b))
                    edgesToBeRemoved.Add(a);
            }
        }
        foreach(Edge removable in edgesToBeRemoved)
            edges.Remove(removable);
    }

    private class Edge
    {
        public int a, b;
        public Edge(int a, int b)
        {
            this.a = a;
            this.b = b;
        }
    }
}