using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

	public Transform player;
	Vector3 offset;

	public void SetOffset () {
		offset = transform.position - player.position;
	}

	void Update () {
		if (player != null) {
			transform.position = offset + player.position;
		}
	}
}
