using UnityEngine;
using System.Collections;

public class ShootingEntety : MonoBehaviour {

	public GameObject projectylePrefab;
	public Transform muzzle;
	public float shootDelay;
	float timeOfLastShot;
	public float projectyleSpeed;

	public bool ReadyToShoot() {
		return timeOfLastShot + shootDelay < Time.time;
	}

	public void Shoot (Vector3 direction) {		
		transform.up = direction;
		Fire();
		timeOfLastShot = Time.time;
	}

	public void Look (Vector3 direction) {
		transform.up = direction;
	}
		
	void Fire() {
		var newProjectyle = (GameObject) Instantiate (projectylePrefab, muzzle.position, Quaternion.identity);
		newProjectyle.transform.up = transform.up;

		Rigidbody2D projRB = newProjectyle.GetComponent <Rigidbody2D> ();
		if (projRB != null) {
			projRB.velocity = transform.up * projectyleSpeed;
		}

	}
}
