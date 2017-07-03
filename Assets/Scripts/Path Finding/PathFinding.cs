using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathFinding : MonoBehaviour {

	PathRequestManager requestManager;
	Grid grid;
	int iterations;


	void Awake () {
		requestManager = GetComponent <PathRequestManager> ();
		grid = GetComponent <Grid> ();
	}


	public void StartFindPath (Vector3 startPos, Vector3 targetPos) {
		StartCoroutine (FindPath (startPos, targetPos));
	}


	IEnumerator FindPath (Vector3 startPos, Vector3 targetPos) {
		iterations = 0;

		Vector2[] wayPoints = new Vector2[0];
		bool pathSuccess = false;

		Node startNode = grid.NodeFromWorldPoint (startPos);
		Node targetNode = grid.NodeFromWorldPoint (targetPos);

		if (!startNode.walkable) {
			startNode = grid.GetWalkabelNeighbours (startNode) [0];
		}

		if (targetNode.walkable) {

			Heap <Node> openSet = new Heap<Node> (grid.MaxSize);
			HashSet <Node> closeSet = new HashSet<Node> ();
			openSet.Add (startNode);

			while (openSet.Count > 0) {
				Node currentNode = openSet.RemoveFirstItem ();
				closeSet.Add (currentNode);

				if (currentNode == targetNode) {
					pathSuccess = true;
					RetracePath (startNode, targetNode);
					break;
				}

				foreach (Node neighbour in grid.GetWalkabelNeighbours (currentNode)) {
					if (closeSet.Contains (neighbour)) {
						continue;
					}

					int newMovemntCostToNeighbour = currentNode.gCost + GetDistance (currentNode, neighbour);
					if (newMovemntCostToNeighbour < neighbour.gCost || !openSet.Contains (neighbour)) {
						neighbour.gCost = newMovemntCostToNeighbour;
						neighbour.parent = currentNode;

						if (!openSet.Contains (neighbour)) {
							neighbour.hCost = GetDistance (neighbour, targetNode); //if there is problem, it might be here
							openSet.Add (neighbour);
						} else {
							openSet.UpdateItem (neighbour);
						}
					}
				}

				iterations++;
				if (iterations > 500) {
					iterations = 0;
					yield return null;
				}
			}
		}

		yield return null;
		if (pathSuccess) {
			wayPoints = RetracePath (startNode, targetNode);
		}
		requestManager.FinishedProccesingPath (wayPoints, pathSuccess);
	}


	Vector2[] RetracePath (Node startNode, Node endNode) {
		List<Node> path = new List<Node> ();
		Node currentNode = endNode;

		path.Add (currentNode);
		while (currentNode != startNode) {
			currentNode = currentNode.parent;
			path.Add (currentNode);
		} 

//		grid.path = path;

		Vector2[] waypoints = SimplifyPath (path);
		Array.Reverse (waypoints);
		return waypoints;
	}

	Vector2[] SimplifyPath (List<Node> path) {
		List <Vector2> waypoints = new List<Vector2> ();
		Vector2 directionOld = Vector2.zero;

		for (int i = 1; i < path.Count; i++) {
			Vector2 directionNew = new Vector2 (path [i].gridX - path [i-1].gridX, path [i].gridY - path [i-1].gridY); 
			if (directionNew != directionOld) {
				waypoints.Add (path[i-1].worldPostion);
			}
			directionOld = directionNew;
		}

		return waypoints.ToArray ();
	}

	int GetDistance (Node nodeA, Node nodeB) {
		int dstX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs (nodeA.gridY - nodeB.gridY);

		if (dstX > dstY) {
			return 14 * dstY + 10 * (dstX - dstY);
		} else {
			return 14 * dstX + 10 * (dstY - dstX);
		}
	}
}
