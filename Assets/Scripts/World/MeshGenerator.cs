using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshGenerator : MonoBehaviour 
{
    private readonly float SQUARE_SIZE = 1f;
    private SquareGrid squareGrid;
    private List<Vector3> vertices;
    private List<int> triangles;

    public Mesh GenerateMesh(int[,] map)
    {
        squareGrid = new SquareGrid(map);
        vertices = new List<Vector3>();
        triangles = new List<int>();
        for(int i = 0; i < squareGrid.squares.GetLength(1); i++)
        {
            for(int j = 0; j < squareGrid.squares.GetLength(0); j++)
            {
                TriangulateSquare(squareGrid.squares[i, j]);
            }
        }
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }

    private void TriangulateSquare(Square square)
    {
        switch(square.configuration)
        {
            //0 points
            case 0:
                break;
            //1 points
            case 1:
                MeshFromPoints(square.centerBottom, square.bottomLeft, square.centerLeft);
                break;
            case 2:
                MeshFromPoints(square.centerRight, square.bottomRight, square.centerBottom);
                break;
            case 4:
                MeshFromPoints(square.centerTop, square.topRight, square.centerRight);
                break;
            case 8:
                MeshFromPoints(square.centerLeft, square.topLeft, square.centerTop);
                break;
            //2 points
            case 3:
                MeshFromPoints(square.centerRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 5:
                MeshFromPoints(square.bottomLeft, square.centerLeft, square.centerTop, square.topRight, square.centerRight, square.centerBottom);
                break;
            case 6:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.centerBottom);   
                break;
            case 9:
                MeshFromPoints(square.bottomLeft, square.topLeft, square.centerTop, square.centerBottom);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerLeft);
                break;
            //3 points
            case 7:
                MeshFromPoints(square.centerLeft, square.centerTop, square.topRight, square.bottomRight, square.bottomLeft);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;
            //4 points
            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                break;
            default:
                Debug.Log("Unidentified Configuration!");
                break;
        }
    }

    private void MeshFromPoints(params Node[] points)
    {
        AssignVertices(points);
        switch(points.Length)
        {
            case 6:
                CreateTriangle(points[0], points[4], points[5]);
                goto case 5;
            case 5:
                CreateTriangle(points[0], points[3], points[4]);
                goto case 4;
            case 4:
                CreateTriangle(points[0], points[2], points[3]);
                goto case 3;
            case 3:
                CreateTriangle(points[0], points[1], points[2]);
                break;
            default:
                Debug.Log("Unidentified Points!");
                break;
        }
    }

    private void AssignVertices(Node[] points)
    {
        for(int i = 0; i < points.Length; i++)
        {
            if(points[i].vertexIndex == -1)
            {
                points[i].vertexIndex = vertices.Count;
                vertices.Add(points[i].position);
            }
        }
    }

    private void CreateTriangle(Node a, Node b, Node c)
    {
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);
    }

    // void OnDrawGizmos()
    // {
    //     if(squareGrid != null)
    //     {
    //         for(int i = 0; i < squareGrid.squares.GetLength(1); i++)
    //         {
    //             for(int j = 0; j < squareGrid.squares.GetLength(0); j++)
    //             {
    //                 Gizmos.color = (squareGrid.squares[i,j].topLeft.active)?Color.red:Color.white;
    //                 Gizmos.DrawCube(squareGrid.squares[i,j].topLeft.position, Vector3.one * .4f);

    //                 Gizmos.color = (squareGrid.squares[i,j].topRight.active)?Color.black:Color.white;
    //                 Gizmos.DrawCube(squareGrid.squares[i,j].topRight.position, Vector3.one * .4f);

    //                 Gizmos.color = (squareGrid.squares[i,j].bottomRight.active)?Color.black:Color.white;
    //                 Gizmos.DrawCube(squareGrid.squares[i,j].bottomRight.position, Vector3.one * .4f);

    //                 Gizmos.color = (squareGrid.squares[i,j].bottomLeft.active)?Color.black:Color.white;
    //                 Gizmos.DrawCube(squareGrid.squares[i,j].bottomLeft.position, Vector3.one * .4f);


    //                 Gizmos.color = Color.grey;
    //                 Gizmos.DrawCube(squareGrid.squares[i,j].centerTop.position, Vector3.one * .15f);
    //                 Gizmos.DrawCube(squareGrid.squares[i,j].centerRight.position, Vector3.one * .15f);
    //                 Gizmos.DrawCube(squareGrid.squares[i,j].centerBottom.position, Vector3.one * .15f);
    //                 Gizmos.DrawCube(squareGrid.squares[i,j].centerLeft.position, Vector3.one * .15f);
    //             }
    //         }
    //     }
    // }

    private class SquareGrid
    {
        public Square[,] squares;

        public SquareGrid(int[,] map)
        {
            int nodeCols = map.GetLength(0);
            int nodeRows = map.GetLength(1);
            float mapWidth = nodeCols*SQUARE_SIZE;
            float mapHeight = nodeRows*SQUARE_SIZE;
            ControlNode[,] controlNodes = new ControlNode[nodeCols, nodeRows];
            for(int i = 0; i < nodeRows; i++)
            {
                for(int j = 0; j < nodeCols; j++)
                {
                    Vector3 position = new Vector3(-mapWidth/2 + j*SQUARE_SIZE + SQUARE_SIZE/2, mapHeight/2 - i*SQUARE_SIZE - SQUARE_SIZE/2, 0);
                    controlNodes[i, j] = new ControlNode(position, map[i, j] == 1);
                }
            }
            int rows = nodeRows-1;
            int cols = nodeCols-1;
            squares = new Square[rows, cols];
            for(int i = 0; i < rows; i++)
            {
                for(int j = 0; j < cols; j++)
                {
                    squares[i, j] = new Square(controlNodes[i, j], controlNodes[i, j+1], controlNodes[i+1, j+1], controlNodes[i+1, j]);
                }
            }
        }
    }

    private class Square
    {
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centerTop, centerRight, centerBottom, centerLeft;
        public int configuration; //topLeft:topRight:bottomRight:bottomLeft for bit order

        public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomRight, ControlNode bottomLeft)
        {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomRight = bottomRight;
            this.bottomLeft = bottomLeft;
            centerTop = topLeft.right;
            centerRight = bottomRight.above;
            centerBottom = bottomLeft.right;
            centerLeft = bottomLeft.above;
            if(topLeft.active)
                configuration += 8;
            if(topRight.active)
                configuration += 4;
            if(bottomRight.active)
                configuration += 2;
            if(bottomLeft.active)
                configuration += 1;
        }
    }

    private class Node
    {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 position)
        {
            this.position = position;
        }
    }

    private class ControlNode : Node
    {
        public bool active;
        public Node above, right;

        public ControlNode(Vector3 position, bool active) : base(position)
        {
            this.active = active;
            this.above = new Node(position + Vector3.up*SQUARE_SIZE/2f);
            this.right = new Node(position + Vector3.right*SQUARE_SIZE/2f);
        }
    }
}