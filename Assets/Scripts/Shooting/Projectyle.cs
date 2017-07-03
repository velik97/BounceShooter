using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TrailRenderer))]
public class Projectyle : MonoBehaviour {

	public int lives = 5;
	float trailLengthInTime;

	void Awake () {
		trailLengthInTime = GetComponent <TrailRenderer> ().time;
	}

	void OnCollisionEnter2D (Collision2D other) {
		if (other.gameObject.tag == "Player" || other.gameObject.tag == "Enemy")
			lives = 0;
		else
			lives -= 1;
		
		if (lives <= 0) {
			Destroy (this.gameObject, trailLengthInTime);
			PsudoDisactivate ();
		}
	}

	void PsudoDisactivate () {
		Destroy (GetComponent <Rigidbody2D> ());
		Destroy (GetComponent <CircleCollider2D> ());
		Destroy (GetComponent <SpriteRenderer> ());
	}
}
