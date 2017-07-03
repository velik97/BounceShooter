using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(FieldOfView))]
public class EnemyShootController : ShootController {

	public Transform target;
	public MovingEntety targetMovingEntity;
	public FieldOfView fow;
//	Vector3 point;
//	LiveSystem liveSystem;

	public bool smartShooting;

	protected override void Awake () {
		base.Awake ();
		fow = GetComponent <FieldOfView> ();
		target = GameObject.FindWithTag ("Player").transform;
	}

	void Update () {
		if (fow.playerIsVisible && target != null) {
			Vector3 direction;

			if (smartShooting) {
				Vector2 myPos = (Vector2)(transform.position);
				Vector2 tarPos = (Vector2)(target.position);
				Vector2 tarVel = targetMovingEntity.velocity;
		
			
				Vector2 optimalDirection = FindOptimalDirection (myPos, tarPos,
					shootingEntity.projectyleSpeed, tarVel);
				direction = (Vector3)optimalDirection;

			} else {
				direction = target.position - transform.position;
			}

			if (shootingEntity.ReadyToShoot ()) {
				Shoot (direction);
			} else {
				Look (direction);
			}
		}
	}
		

	Vector2 FindOptimalDirection (Vector2 myPos, Vector2 tarPos, float prVel, Vector2 tarVel) {

		// Quadratic equation coefficients
		float a = prVel * prVel - tarVel.sqrMagnitude;

		if (a <= 0) {
			return tarPos - myPos;
		}

		float halfB = (myPos.x - tarPos.x) * tarVel.x + (myPos.y - tarPos.y) * tarVel.y;
		float c = (2f * (myPos.x * tarPos.x + myPos.y * tarPos.y) - myPos.sqrMagnitude - tarPos.sqrMagnitude);

		float t;


		t = (Mathf.Sqrt (halfB * halfB - a * c) - halfB) / a;

		Vector2 velocity = new Vector2 (((tarPos.x - myPos.x)/t) + tarVel.x,
			((tarPos.y - myPos.y)/t) + tarVel.y);

		return velocity;
	}

//	void OnDrawGizmos () {
//		if (point != null) {
//			Gizmos.color = Color.red;
//			Gizmos.DrawCube (point, Vector3.one * .5f);
//		}
//	}
		
}
