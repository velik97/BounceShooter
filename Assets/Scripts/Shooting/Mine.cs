using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ExplodingEntity))]
public class Mine : MonoBehaviour {

	public float activateTime = 2f;
	bool activated;
	ExplodingEntity explodingEntity;

	void Awake () {
		explodingEntity = GetComponent <ExplodingEntity> ();
		activated = false;
		Invoke ("Activate", activateTime);
	}

	void Activate () {
		activated = true;
	}

	void OnTriggerEnter2D () {
		if (activated) {activated = false;
			explodingEntity.Explode ();
		}
	}

}
