using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshGenerator : MonoBehaviour {

	public float wallHeight = 5f;

	public SquareGrid squareGrid;
    public MeshFilter cave;
	public PhysicsMaterial2D bouncy;

	List <Vector3> vertices;
	List <int> triangles;

	Dictionary <int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>> ();
	List<List<int>> outlines = new List<List<int>> ();
	HashSet <int> checkedVertices = new HashSet<int> ();

	public void GenerateMesh (int [,] map, float squareSize) {

		triangleDictionary.Clear ();
		outlines.Clear ();
		checkedVertices.Clear ();

		squareGrid = new SquareGrid (map, squareSize);

		vertices = new List<Vector3> ();
		triangles = new List<int> ();

		for (int x = 0; x < squareGrid.squares.GetLength (0); x++) {
			for (int y = 0; y < squareGrid.squares.GetLength (1); y++) {
				TriangulateSquare (squareGrid.squares [x, y]);
			}
		}

		Mesh mesh = new Mesh ();
		cave.mesh = mesh;

		mesh.vertices = vertices.ToArray ();
		mesh.triangles = triangles.ToArray ();
		mesh.RecalculateNormals ();


		CreateCollider2D ();
	}
		

	void CreateCollider2D () {
		EdgeCollider2D[] currentColliders = gameObject.GetComponents <EdgeCollider2D> ();
		for (int i = 0; i < currentColliders.Length; i++) {
			Destroy (currentColliders [i]);
		}

		CalculateMeshOutLines ();

		foreach (List <int> outline in outlines) {
			EdgeCollider2D edgeCollider = gameObject.AddComponent <EdgeCollider2D> ();
			edgeCollider.sharedMaterial = bouncy;
			Vector2[] edgePoints = new Vector2[outline.Count];
			for (int i = 0; i < edgePoints.Length; i++) {
				edgePoints [i] = new Vector2(vertices [outline[i]].x, vertices [outline[i]].z);
			}
			edgeCollider.points = edgePoints;
		}
	}

	void TriangulateSquare (Square square) {
		switch (square.configuration) {
		case 0:
			break;

		// 1 point:
		case 1:
			MeshFromPoints (square.centerBottom, square.bottomLeft, square.centerLeft);
			break;
		case 2:
			MeshFromPoints (square.centerRight, square.bottomRight, square.centerBottom );
			break;
		case 4:
			MeshFromPoints (square.topRight, square.centerRight, square.centerTop);
			break;
		case 8:
			MeshFromPoints (square.topLeft, square.centerTop, square.centerLeft);
			break;

		// 2 points:
		case 3:
			MeshFromPoints (square.centerRight, square.bottomRight, square.bottomLeft, square.centerLeft);
			break;
		case 6:
			MeshFromPoints (square.centerTop, square.topRight, square.bottomRight, square.centerBottom);
			break;
		case 9:
			MeshFromPoints (square.topLeft, square.centerTop, square.centerBottom, square.bottomLeft);
			break;
		case 12:
			MeshFromPoints (square.topLeft, square.topRight, square.centerRight , square.centerLeft);
			break;
		case 5:
			MeshFromPoints (square.centerTop, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft, square.centerLeft);
			break;
		case 10:
			MeshFromPoints (square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.centerBottom, square.centerLeft);
			break;

		// 3 points
		case 7:
			MeshFromPoints (square.centerTop, square.topRight, square.bottomRight, square.bottomLeft, square.centerLeft);
			break;
		case 11:
			MeshFromPoints (square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft );
			break;
		case 13:
			MeshFromPoints (square.topLeft, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft);
			break;
		case 14:
			MeshFromPoints (square.topLeft, square.topRight, square.bottomRight, square.centerBottom, square.centerLeft);
			break;
		
		// 4 points:
		case 15:
			MeshFromPoints (square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
			checkedVertices.Add (square.topLeft.vertexIndex);
			checkedVertices.Add (square.topRight.vertexIndex);
			checkedVertices.Add (square.bottomRight.vertexIndex);
			checkedVertices.Add (square.bottomLeft.vertexIndex);
			break;

		}
	}

	void MeshFromPoints (params Node[] points) {
		AssignVertices (points);

		if (points.Length >= 3)
			CreateTriangle (points [0], points [1], points [2]);
		if (points.Length >= 4)
			CreateTriangle (points [0], points [2], points [3]);
		if (points.Length >= 5)
			CreateTriangle (points [0], points [3], points [4]);
		if (points.Length >= 6)
			CreateTriangle (points [0], points [4], points [5]);

	}

	void AssignVertices (Node[] points) {
		for (int i = 0; i < points.Length; i++) {
			if (points [i].vertexIndex == -1) {
				points [i].vertexIndex = vertices.Count;
				vertices.Add (points [i].position);
			}
		}
	}

	void CreateTriangle (Node a, Node b, Node c) {
		triangles.Add (a.vertexIndex);
		triangles.Add (b.vertexIndex);
		triangles.Add (c.vertexIndex);

		Triangle triangle = new Triangle (a.vertexIndex, b.vertexIndex, c.vertexIndex);

		AddTriangleToDictionary (triangle.vertexIndexA, triangle);
		AddTriangleToDictionary (triangle.vertexIndexB, triangle);
		AddTriangleToDictionary (triangle.vertexIndexC , triangle);
	}

	void AddTriangleToDictionary (int vertexIndexKey, Triangle triangle) {
		if (triangleDictionary.ContainsKey (vertexIndexKey)) {
			triangleDictionary [vertexIndexKey].Add (triangle);
		} else {
			List <Triangle> triangleList = new List<Triangle> ();
			triangleList.Add (triangle);
			triangleDictionary.Add (vertexIndexKey, triangleList);
		}
	}

	void CalculateMeshOutLines () {
		for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++) {
			if (!checkedVertices.Contains (vertexIndex)) {
				int newOutlineVertex = GetConnectedOutlineVertex (vertexIndex);
				if (newOutlineVertex != -1) {
					checkedVertices.Add (vertexIndex);

					List <int> newOutline = new List<int> ();
					newOutline.Add (vertexIndex);
					outlines.Add (newOutline);
					FollowOutline (newOutlineVertex, outlines.Count - 1);
					outlines [outlines.Count - 1].Add (vertexIndex);
				}
			}
		}
	}

	void FollowOutline (int vertexIndex, int outlineIndex) {
		outlines [outlineIndex].Add (vertexIndex);
		checkedVertices.Add (vertexIndex);
		int nextVertexIndex = GetConnectedOutlineVertex (vertexIndex);

		if (nextVertexIndex != -1) {
			FollowOutline (nextVertexIndex, outlineIndex);
		}
	}

	int GetConnectedOutlineVertex (int vertexIndex) {
		List <Triangle> trianglesContainigVertex = triangleDictionary [vertexIndex];

		for (int i = 0; i < trianglesContainigVertex.Count; i++) {
			Triangle triangle = trianglesContainigVertex [i];

			for (int j = 0; j < 3; j++) {
				int vertexB = triangle[j];
				if (vertexB != vertexIndex && !checkedVertices.Contains (vertexB))
					if (isOutlineEdge (vertexIndex, vertexB))
						return vertexB; 
			}
		}

		return -1;
	}

	bool isOutlineEdge (int vertexA, int vertexB) {
		List <Triangle> trianglesContainingVertexA = triangleDictionary [vertexA];

		int sharedTriangleCount = 0;

		for (int i = 0; i < trianglesContainingVertexA.Count; i++) {
			if (trianglesContainingVertexA [i].Contains (vertexB))
				sharedTriangleCount++;
			if (sharedTriangleCount > 1)
				break;
		}

		return sharedTriangleCount == 1;
	}

	struct Triangle {
		public int vertexIndexA;
		public int vertexIndexB;
		public int vertexIndexC;
		int[] vertices;

		public Triangle (int a, int b, int c) {
			vertexIndexA = a;
			vertexIndexB = b;
			vertexIndexC = c;

			vertices = new int[3];
			vertices[0] = a;
			vertices[1] = b;
			vertices[2] = c;
		}

		public int this [int i] {
			get {
				return vertices [i];
			}
		}

		public bool Contains (int VertexIndex) {
			return (VertexIndex == vertexIndexA || VertexIndex == vertexIndexB || VertexIndex == vertexIndexC);
		}
	}

	public class SquareGrid {
		public Square[,] squares;

		public SquareGrid (int [,] map, float squareSize) {
			int nodeCountX = map.GetLength (0);
			int nodeCountY = map.GetLength (1);

			float mapWidth = nodeCountX * squareSize;
			float mapHeight = nodeCountY * squareSize;

			ControllNode[,] controllNodes = new ControllNode[nodeCountX,nodeCountY];

			for (int x = 0; x < nodeCountX; x ++) {
				for (int y = 0; y < nodeCountY; y ++) {
					Vector3 position = new Vector3 (-mapWidth * .5f + x * squareSize + squareSize * .5f, 0 ,-mapHeight * .5f + y * squareSize + squareSize * .5f);
					controllNodes [x,y] = new ControllNode (position, map [x,y] == 1, squareSize);
				}
			}

			squares = new Square[nodeCountX - 1, nodeCountY - 1];

			for (int x = 0; x < nodeCountX - 1; x ++) {
				for (int y = 0; y < nodeCountY - 1; y ++) {
					squares[x,y] = new Square (controllNodes [x, y+1], controllNodes [x+1 , y+1], controllNodes[x+1, y], controllNodes[x,y]);
				}
			}
		}
	}

	public class Square {
		public ControllNode topLeft, topRight, bottomLeft, bottomRight;
		public Node centerTop, centerBottom, centerRight, centerLeft;
		public int configuration;

		public Square (ControllNode _topLeft, ControllNode _topRight, ControllNode _bottomRight, ControllNode _bottomLeft) {
			topLeft = _topLeft;
			topRight = _topRight;
			bottomRight = _bottomRight;
			bottomLeft = _bottomLeft;

			centerTop = topLeft.right;
			centerBottom = bottomLeft.right;
			centerRight = bottomRight.above;
			centerLeft = bottomLeft.above;

			configuration = 0;
			if (topLeft.active)
				configuration += 8;
			if (topRight.active)
				configuration += 4;
			if (bottomRight.active)
				configuration += 2;
			if (bottomLeft.active)
				configuration += 1;
		}
	}

	public class Node {
		public Vector3 position;
		public int vertexIndex = -1;

		public Node (Vector3 _pos) {
			position = _pos;
		}
	}


	public class ControllNode : Node {
		public bool active;
		public Node above, right;

		public ControllNode (Vector3 _pos, bool _active, float sqareSize) : base(_pos) {
			active = _active;
			above = new Node (position + Vector3.forward * sqareSize / 2f);
			right = new Node (position + Vector3.right * sqareSize / 2f);
		}
	}
}
