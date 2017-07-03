using UnityEngine;
using System.Collections;
using CnControls;

public class PlayerMoveController : MoveController {

	public bool useJoysticks;

	void Update () {
		float hor, ver;

		hor = CnInputManager.GetAxisRaw ("Horizontal");
		ver = CnInputManager.GetAxisRaw ("Vertical");

		movement = new Vector2 (hor, ver);
	}
		
}
