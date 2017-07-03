using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingEntety : MonoBehaviour {

	public float speed;
	public Vector2 velocity;
	Rigidbody2D rb;

	Vector2 currentPos;
	Vector2 prevPos;
	void Awake () {
		rb = GetComponent <Rigidbody2D> ();

		currentPos = Vector2.zero;
		prevPos = Vector2.zero;
	}
		
	void Update () {
		currentPos = transform.position;
		velocity = (currentPos - prevPos) / Time.deltaTime;
		prevPos = currentPos;
	}
	
	public void Move (Vector2 movement) {
		rb.AddForce (movement * speed * Time.fixedDeltaTime, ForceMode2D.Impulse);
	}
}
