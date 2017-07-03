using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathRequestManager : MonoBehaviour {

	Queue<PathRequest> pathRequestQueue = new Queue<PathRequest> ();
	PathRequest currentPathRequest;

	static PathRequestManager instance;
	PathFinding pathFinding;
	bool isProcessinsPath;

	void Awake () {
		instance = this;
		pathFinding = GetComponent <PathFinding> ();
	}

	public static void RequestPath (Vector3 pathStart, Vector3 pathEnd, Action<Vector2[], bool> callback) {
		PathRequest newRequest = new PathRequest (pathStart, pathEnd, callback);

		instance.pathRequestQueue.Enqueue (newRequest);
		instance.TryProcessNext ();
	}

	void TryProcessNext() {
		if (!isProcessinsPath && pathRequestQueue.Count > 0) {
			currentPathRequest = pathRequestQueue.Dequeue ();
			isProcessinsPath = true;
			pathFinding.StartFindPath (currentPathRequest.pathStart, currentPathRequest.pathEnd);
		}
	}

	public void FinishedProccesingPath (Vector2[] path, bool success) {
		if (currentPathRequest.callback != null)
			currentPathRequest.callback (path, success);
		isProcessinsPath = false;
		TryProcessNext ();
	}

	struct PathRequest {
		public Vector3 pathStart;
		public Vector3 pathEnd;
		public Action<Vector2[], bool> callback;

		public PathRequest (Vector3 _start, Vector3 _end, Action<Vector2[], bool> _callback) {
			pathStart = _start;
			pathEnd = _end;
			callback = _callback;
		}
	}
}
