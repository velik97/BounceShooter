using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour {

	public float viewDistance;
	[Range(0,360)]
	public float viewAngle;
	float halfOfViewAngle;

	public LayerMask targetMask; 
	public LayerMask obstacleMask;
	public Transform targetTransform;
	public bool targetIsVisible;
	public bool playerIsVisible;

	public float meshResolution;
	public int EdgeResolveIterations;
	public float edgeDistanceThreshold;

	public MeshFilter viewMeshFilter;
	Mesh viewMesh;

	List <EdgeInfo> edgeInfos;

	void Start () {
		playerIsVisible = false;
		targetIsVisible = false;
		viewMesh = new Mesh ();
		viewMesh.name = "View Mesh";
		viewMeshFilter.mesh = viewMesh;
		halfOfViewAngle = viewAngle * 0.5f;
		StartCoroutine (CheckPkayer ());
	}

	void LateUpdate () {
		DrawFieldOfView ();
	}

	IEnumerator CheckPkayer () {
		while (true) {
			TargetIsInFieldOfView ();
			yield return new WaitForSeconds (.1f);
		}
	}

	void TargetIsInFieldOfView () {
		targetIsVisible = false;
		playerIsVisible = false;
		Collider2D[] targetColliders = Physics2D.OverlapCircleAll (transform.position, viewDistance, targetMask);
		foreach (Collider2D col in targetColliders) {
			targetTransform = col.transform;
			
			Vector2 dirToTarget = (Vector2)(targetTransform.position - transform.position);
			float angleToTarget = Vector2.Angle (transform.up, dirToTarget);
			float distToTarget = Vector2.Distance (targetTransform.position, transform.position);
			RaycastHit2D hitToTarget = Physics2D.Raycast (transform.position, dirToTarget, distToTarget, obstacleMask);

			if (targetTransform != null && angleToTarget < halfOfViewAngle && hitToTarget.transform == null) {
				targetIsVisible = true;
				if (targetTransform.tag == "Player") {
					playerIsVisible = true;
					return;
				}
			}
		}
	}

	void DrawFieldOfView () {
		int stepCount = Mathf.RoundToInt (viewAngle * meshResolution);
		float stepAngleSize = viewAngle / stepCount;
		List<Vector3> viewPoints = new List<Vector3> ();
		ViewCastInfo oldViewCast = new ViewCastInfo ();

		edgeInfos = new List<EdgeInfo> (); 

		float offsetAngle = -transform.eulerAngles.z - viewAngle / 2;
		for (int i = 0; i <= stepCount; i++) {
			float angle = offsetAngle + stepAngleSize * i;
			ViewCastInfo newViewCast = ViewCast (angle);

			if (i > 0) {
				bool edgeDistanceThersholdExceeded = Mathf.Abs (newViewCast.distance - oldViewCast.distance) > edgeDistanceThreshold;
				if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThersholdExceeded)) {
					EdgeInfo edge = FindEdge (oldViewCast, newViewCast);
					if (edge.pointA != Vector3.zero) {
						viewPoints.Add (edge.pointA);
					}
					if (edge.pointB != Vector3.zero) {
						viewPoints.Add (edge.pointB);
					}
					edgeInfos.Add (edge);
				}
			}

			viewPoints.Add ((Vector3)newViewCast.point);
			oldViewCast = newViewCast;
		}

		int vertexCount = viewPoints.Count + 1;
		Vector3[] vertices = new Vector3[vertexCount];
		int[] triangles = new int[(vertexCount - 2) * 3];

		vertices [0] = Vector3.zero;
		for (int i = 0; i < vertexCount - 1; i++) {
			vertices [i+1] = transform.InverseTransformPoint (viewPoints[i]);

			if (i < vertexCount - 2) {
				triangles [i * 3] = 0;
				triangles [i * 3 + 1] = i + 1;
				triangles [i * 3 + 2] = i + 2;
			}
		}

		viewMesh.Clear ();
		viewMesh.vertices = vertices;
		viewMesh.triangles = triangles;
		viewMesh.RecalculateNormals ();
	}

	ViewCastInfo ViewCast (float globalAngle) {
		Vector3 direction = (Vector3)DirectionFromAngle (globalAngle, true);
		RaycastHit2D hit = Physics2D.Raycast (transform.position, direction, viewDistance, obstacleMask);
		if (hit.transform == null) {
			return new ViewCastInfo (false, transform.position + direction * viewDistance, globalAngle, viewDistance);
		} else {
			return new ViewCastInfo (true, (Vector3)hit.point, globalAngle, hit.distance);
		}
	}

	public Vector2 DirectionFromAngle (float angleInDegrees, bool angleIsGlobal) {
		if (!angleIsGlobal) {
			angleInDegrees -= transform.eulerAngles.z;
		}
		return new Vector2 (Mathf.Sin (angleInDegrees * Mathf.Deg2Rad), Mathf.Cos (angleInDegrees * Mathf.Deg2Rad));
	}

	EdgeInfo FindEdge (ViewCastInfo minViewCast, ViewCastInfo maxViewCast) {
		float minAngle = minViewCast.angle;
		float maxAngle = maxViewCast.angle;
		Vector3 minPoint = Vector3.zero;
		Vector3 maxPoint = Vector3.zero;

		for (int i = 0; i < EdgeResolveIterations; i++) {
			float angle = (minAngle + maxAngle) / 2f;
			ViewCastInfo newViewCast = ViewCast (angle);

			bool edgeDistanceThersholdExceeded = Mathf.Abs (newViewCast.distance - minViewCast.distance) > edgeDistanceThreshold;
			if (newViewCast.hit == minViewCast.hit && !edgeDistanceThersholdExceeded) {
				minAngle = angle;
				minPoint = newViewCast.point;
			} else {
				maxAngle = angle;
				maxPoint = newViewCast.point;
			}
		}

		return new EdgeInfo (minPoint, maxPoint);

	}

	public struct ViewCastInfo {
		public bool hit;
		public Vector3 point;
		public float angle;
		public float distance;

		public ViewCastInfo(bool _hit, Vector3 _point, float _angle, float _distance) {
			hit = _hit;
			point = _point;
			angle = _angle;
			distance = _distance;
		}
	}

	public struct EdgeInfo {
		public Vector3 pointA;
		public Vector3 pointB;

		public EdgeInfo (Vector3 _pointA, Vector3 _pointB) {
			pointA = _pointA;
			pointB = _pointB;
		}
	}

	void OnDrawGismos () {
		if (edgeInfos != null) {
			print ("check");
			foreach (EdgeInfo edge in edgeInfos) {
				Gizmos.color = Color.red;
				Gizmos.DrawSphere (edge.pointA, 1f);
				Gizmos.color = Color.red;
				Gizmos.DrawSphere (edge.pointB, 1f);
			}
		}
	}
}
