using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	public Node[,] grid;
	Vector2 gridWorldSize;
	int gridSizeX, gridSizeY;

//	public List <Node> path; //for Debug
	public static List <Node> walkableNodes;

	public bool displayGridGizmos;

	public int MaxSize {
		get {
			return gridSizeX * gridSizeY;
		}
	}

	public void CreateGrid (MapGenerator mapGen, float squareSize) {
		int[,] map = mapGen.map;

		gridSizeX = map.GetLength (0);
		gridSizeY = map.GetLength (1);

		float mapWidth = gridSizeX * squareSize;
		float mapHeight = gridSizeY * squareSize;

		gridWorldSize = new Vector2 (mapWidth, mapHeight);

		grid = new Node[gridSizeX,gridSizeY];

		walkableNodes = new List<Node> ();
		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {
				Vector2 worldPosition = new Vector2 (-mapWidth * .5f + (x + .5f) * squareSize,-mapHeight * .5f + (y + .5f) * squareSize);
				bool walkable = map [x, y] == 0;
				grid [x,y] = new Node (walkable, worldPosition, x, y);
				if (walkable) {
					walkableNodes.Add (grid [x,y]);
				}
			}
		}
	}

	public Node NodeFromWorldPoint (Vector3 worldPosition) {
		float persentX = (worldPosition.x + gridWorldSize.x / 2f) / gridWorldSize.x;
		float persentY = (worldPosition.y + gridWorldSize.y / 2f) / gridWorldSize.y;

		persentX = Mathf.Clamp01 (persentX);
		persentY = Mathf.Clamp01 (persentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * persentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * persentY);

		return grid [x, y];
	}

	public List<Node> GetWalkabelNeighbours (Node node) {
		List <Node> neighbours = new List<Node> ();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (!(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY))
					continue;

				if (!grid [checkX, checkY].walkable)
					continue;

				if (x != 0 && y != 0) {
					if (!grid [node.gridX, checkY].walkable && !grid [checkX, node.gridY].walkable)
						continue;
				}

				neighbours.Add (grid[checkX, checkY]);
			}
		}
			
		return neighbours;
	}

	public static Vector3 GetRandomWalkablePoint() {
		int randomNodeIndex = Random.Range (0, walkableNodes.Count);
		Vector3 randomPosition = (Vector3)walkableNodes [randomNodeIndex].worldPostion;

		return randomPosition;
	}

	void OnDrawGizmos() {
		if (grid != null && displayGridGizmos) {
			for (int x = 0; x < grid.GetLength (0); x ++) {
				for (int y = 0; y < grid.GetLength (1); y ++) {
					Gizmos.color = (grid[x,y].walkable)?Color.white:Color.black;
//					if (path != null && path.Contains (grid [x, y]))
//						Gizmos.color = Color.red;
					Gizmos.DrawCube((Vector3)grid[x,y].worldPostion,Vector3.one * 2.7f);
				}
			}
		}

	}
}
