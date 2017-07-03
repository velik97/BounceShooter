using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MovingEntety))]
public abstract class MoveController : MonoBehaviour {

	MovingEntety movingEntety;
	protected Vector2 movement;

	protected virtual void Awake () {
		movingEntety = GetComponent <MovingEntety> ();
	}

	void FixedUpdate () {
		movingEntety.Move (movement);
	}

}
