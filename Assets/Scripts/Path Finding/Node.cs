using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node> {

	public bool walkable;
	public Vector2 worldPostion;

	public int gridX;
	public int gridY;

	public int gCost;
	public int hCost;

	public Node parent;

	int heapIndex;

	public Node (bool _walkable, Vector2 _worldPosition, int _gridX, int _gridY) {
		walkable = _walkable;
		worldPostion = _worldPosition;
		gridX = _gridX;
		gridY = _gridY;
	}

	public int fCost {
		get {
			return hCost + gCost;
		}
	}

	public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

	public int CompareTo (Node nodeToCompare) {
		int compare = fCost.CompareTo (nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo (nodeToCompare.hCost);
		}
		return -compare;
	}
}
