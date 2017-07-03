using UnityEngine;
using System.Collections;
using CnControls;

public class PlayerShootController : ShootController {

	public LayerMask floorMask;
	public bool useJoysticks;

	void Update () {
		if (useJoysticks) {
			float hor = CnInputManager.GetAxis ("Shoot Horizontal");
			float ver = CnInputManager.GetAxis ("Shoot Vertical");

			Vector3 direction = new Vector3 (hor, ver, 0);
			if (direction != Vector3.zero) {
				direction.Normalize ();
				Shoot (direction);
			}
		} else {
			
				Vector3 mousePoint = Input.mousePosition;
				Ray camRay = Camera.main.ScreenPointToRay (mousePoint);
				RaycastHit hit;

				Vector3 direction = transform.up;
				if (Physics.Raycast (camRay, out hit, 100f, floorMask)) {
					direction = hit.point - transform.position;
				}
			if (Input.GetMouseButton (0)) {
				Shoot (direction);
			} else {
				Look (direction);
			}
		}
	}
}
