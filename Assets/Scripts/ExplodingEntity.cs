using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ShootingEntety))]
public class ExplodingEntity : MonoBehaviour {

	public int particleCount;
	ShootingEntety shootingEntity;

	void Awake () {
		shootingEntity = GetComponent <ShootingEntety> ();
	}

	public void Explode () {
		StartCoroutine (IExplode ());
	}

	IEnumerator IExplode () {
		float shotStepAngle = 2 * Mathf.PI / (float)particleCount;
		float angle = 0;

		for (int i = 0; i < particleCount; i++) {
			float x = Mathf.Cos (angle);
			float y = Mathf.Sin (angle);
			Vector3 direction = new Vector3 (x, y, 0);
			shootingEntity.Shoot (direction);
			angle += shotStepAngle;
			yield return new WaitForSeconds (.01f);
		}
		Destroy (this.gameObject);
	}
}
