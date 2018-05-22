using UnityEngine;
using System.Collections;

public class MeshGenerator : MonoBehaviour 
{
    public SquareGrid squareGrid;

    public void GenerateMesh(int[,] map, float squareSize)
    {
        squareGrid = new SquareGrid(map, squareSize);
    }

    void OnDrawGizmos()
    {
        if(squareGrid != null)
        {
            for(int i = 0; i < squareGrid.squares.GetLength(1); i++)
            {
                for(int j = 0; j < squareGrid.squares.GetLength(0); j++)
                {
                    Gizmos.color = (squareGrid.squares[i,j].topLeft.active)?Color.black:Color.white;
                    Gizmos.DrawCube(squareGrid.squares[i,j].topLeft.position, Vector3.one * .4f);

                    Gizmos.color = (squareGrid.squares[i,j].topRight.active)?Color.black:Color.white;
                    Gizmos.DrawCube(squareGrid.squares[i,j].topRight.position, Vector3.one * .4f);

                    Gizmos.color = (squareGrid.squares[i,j].bottomRight.active)?Color.black:Color.white;
                    Gizmos.DrawCube(squareGrid.squares[i,j].bottomRight.position, Vector3.one * .4f);

                    Gizmos.color = (squareGrid.squares[i,j].bottomLeft.active)?Color.black:Color.white;
                    Gizmos.DrawCube(squareGrid.squares[i,j].bottomLeft.position, Vector3.one * .4f);


                    Gizmos.color = Color.grey;
                    Gizmos.DrawCube(squareGrid.squares[i,j].centerTop.position, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[i,j].centerRight.position, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[i,j].centerBottom.position, Vector3.one * .15f);
                    Gizmos.DrawCube(squareGrid.squares[i,j].centerLeft.position, Vector3.one * .15f);
                }
            }
        }
    }

    public class SquareGrid
    {
        public Square[,] squares;

        public SquareGrid(int[,] map, float squareSize)
        {
            int nodeCols = map.GetLength(0);
            int nodeRows = map.GetLength(1);
            float mapWidth = nodeCols*squareSize;
            float mapHeight = nodeRows*squareSize;
            ControlNode[,] controlNodes = new ControlNode[nodeCols, nodeRows];
            for(int i = 0; i < nodeRows; i++)
            {
                for(int j = 0; j < nodeCols; j++)
                {
                    Vector3 position =new Vector3(-mapWidth/2 + i*squareSize + squareSize/2, -mapHeight/2 + j*squareSize + squareSize/2, 0);
                    controlNodes[i, j] = new ControlNode(position, map[i, j] == 1, squareSize);
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

    public class Square
    {
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centerTop, centerRight, centerBottom, centerLeft;

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
        }
    }

    public class Node
    {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 position)
        {
            this.position = position;
        }
    }

    public class ControlNode : Node
    {
        public bool active;
        public Node above, right;

        public ControlNode(Vector3 position, bool active, float squareSize) : base(position)
        {
            this.active = active;
            this.above = new Node(position + Vector3.up*squareSize/2f);
            this.right = new Node(position + Vector3.right*squareSize/2f);
        }
    }
}