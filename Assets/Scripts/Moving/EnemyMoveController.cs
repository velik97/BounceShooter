using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(FieldOfView))]
public class EnemyMoveController : MoveController {

	IEnumerator currentAction;

	public Transform player;
	FieldOfView fow;
	Vector2[] path;
	int targetIndex;

	public float walkSpeed;
	public float runSpeed;

	public int smoothDistanceSteps;
	Queue <Vector3> previousPositions;
	Vector3 previousPosition;

	Vector3 targetPosition;

	public bool followingPlayer;
	bool lookingAround;

	MovingEntety movingEntity;

	protected override void Awake () {
		base.Awake ();
		fow = GetComponent <FieldOfView> ();
		movingEntity = GetComponent <MovingEntety> ();
	}

	void Start () {
		followingPlayer = false;
		previousPosition = transform.position;
		previousPositions = new Queue<Vector3> ();
		WalkRandomly ();
	}


	bool playerWasVisiable = false;
	void Update () {
		if (fow.playerIsVisible || fow.targetIsVisible) {
			if (!playerWasVisiable) {
				StartCoroutine (FollowPlayer ()); 
			}
		}

		playerWasVisiable = fow.playerIsVisible || fow.targetIsVisible;
	}

	IEnumerator FollowPlayer () {
		followingPlayer = true;
		while (true) {
			if (player && (fow.playerIsVisible || fow.targetIsVisible)) {
				GoToPlayer ();
			} else {
				break;
			}
			yield return new WaitForSeconds (.5f);
		}
	}


	void WalkRandomly () {
		print ("[" + gameObject.name + "]: trying to walk randomly");
		movingEntity.speed = walkSpeed;
		targetPosition = Grid.GetRandomWalkablePoint ();
		PathRequestManager.RequestPath (transform.position, targetPosition, OnPathFound);
	}

	void GoToPlayer () {
		print ("[" + gameObject.name + "]: trying to go to player");
		movingEntity.speed = runSpeed;
		targetPosition = player.position;
		PathRequestManager.RequestPath (transform.position, targetPosition, OnPathFound);
	}



	public void OnPathFound (Vector2[] newPath, bool pathSuccessful) {
		if (pathSuccessful && newPath.Length > 0) {
			path = newPath;
			StartActionCoroutine (FollowPath ());
		} else {
			if (!pathSuccessful) {
				Debug.LogError ("[" + gameObject.name + "]: path not found"); 
			} else {
				Debug.LogError ("[" + gameObject.name + "]: path is empty");
			}
			WalkRandomly ();
		}
	}

	void StartActionCoroutine (IEnumerator action) {
		if (currentAction != null) {
			StopCoroutine (currentAction);
		}

		currentAction = action;
		StartCoroutine (currentAction);
	}
		

	void ActionIsDone () {
		print ("[" + gameObject.name + "]: action is done"); 
		if (followingPlayer) {
			
			StartActionCoroutine (LookAround ());
		} else {
			
			WalkRandomly ();
		}
	}

	IEnumerator FollowPath() {
		print ("[" + gameObject.name + "]: started following the path");
		targetIndex = 0;
		Vector2 currentWaypoint = path[targetIndex];
		movement = (currentWaypoint - (Vector2)transform.position).normalized;

		while (true) {
			if ((transform.position - (Vector3)currentWaypoint).sqrMagnitude < 1.5f) {
				
				targetIndex++;
				if (targetIndex >= path.Length) {
					
					movement = Vector2.zero;
					break;
				}
				currentWaypoint = path [targetIndex];
			}
			movement = (currentWaypoint - (Vector2)transform.position).normalized;
			yield return null;
		}

		currentAction = null;

		ActionIsDone ();
	}

	IEnumerator LookAround () {
		print ("[" + gameObject.name + "]: started looking around");
		lookingAround = true;

		float startAngle = -transform.eulerAngles.z;
		float angle = -transform.eulerAngles.z;

		while (true) {
			
			transform.up = fow.DirectionFromAngle (angle, true);
			angle += Time.deltaTime * 40f;

			if (angle - startAngle >= 360) {
				break;
			}
			yield return null;
		}

		currentAction = null;
		lookingAround = false;
		followingPlayer = false;

		ActionIsDone ();

	}



	// Correcting view direction

	void LateUpdate () {
		MakeSmoothForwardDirection ();
	}

	void MakeSmoothForwardDirection () {
		if (!lookingAround) {
			previousPositions.Enqueue (transform.position);

			if (previousPositions.Count > smoothDistanceSteps) {
				previousPosition = previousPositions.Dequeue ();
			}

			if (player && fow.playerIsVisible) {
				transform.up = transform.position - player.position;
			} else {
				transform.up = transform.position - previousPosition;
			}
		} 
	}

	void OnDrawGizmos () {
		if (path != null) { 
			if (path.Length > 0 && targetIndex < path.Length) {
				Gizmos.color = Color.red;
				Gizmos.DrawLine (transform.position, (Vector3)path [targetIndex]);
				Gizmos.color = Color.red;
				Gizmos.DrawCube ((Vector3)path [targetIndex], Vector3.one * 0.5f);
				for (int i = targetIndex; i < path.Length - 1; i++) {
					Gizmos.color = Color.red;
					Gizmos.DrawLine ((Vector3)path [i], (Vector3)path [i + 1]);
					Gizmos.color = Color.red;
					Gizmos.DrawCube ((Vector3)path [i], Vector3.one * 0.5f);
				}
				Gizmos.color = Color.red;
				Gizmos.DrawCube ((Vector3)path [0], Vector3.one);
			}
		}

		if (targetPosition != null) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawCube (targetPosition, Vector3.one);
		}
	}

}
