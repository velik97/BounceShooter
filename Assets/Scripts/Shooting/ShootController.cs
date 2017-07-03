using UnityEngine;
using System.Collections;
using System.CodeDom;

[RequireComponent(typeof(ShootingEntety))]
public abstract class ShootController : MonoBehaviour {

	protected ShootingEntety shootingEntity;

	protected virtual void Awake () {
		shootingEntity = GetComponent <ShootingEntety> ();
	}

	protected void Shoot (Vector3 direction) {
		if (shootingEntity.ReadyToShoot ())
			shootingEntity.Shoot (direction);
	}

	protected void Look (Vector3 direction) {
		shootingEntity.Look (direction);
	}
}
