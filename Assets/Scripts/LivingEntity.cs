using UnityEngine;
using System.Collections;

public class LivingEntity : MonoBehaviour {

	public int startLives = 10;
	public int lives;
	public bool isAlive;
	ExplodingEntity explodingEntity;

	void Awake () {
		lives = startLives;
		isAlive = true;
		explodingEntity = GetComponent <ExplodingEntity> ();
	}

	void OnCollisionEnter2D (Collision2D other) {
		if (other.transform.tag == "Projectyle") {
			lives--;
			if (lives <= 0) {
				Die ();
			}
		}
	}

	void Die () {
		if (isAlive) {
			print (tag + " is Dead");
			explodingEntity.Explode ();
		}
		isAlive = false;
	}

}
