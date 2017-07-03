using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour {

	Transform player;

	void Awake () {
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent <Transform> ();
	}

	public void Start () {
		PutPlayerInRandomPosition ();
	}

	void PutPlayerInRandomPosition () {
		Vector3 randomPosition = Grid.GetRandomWalkablePoint ();

		player.position = randomPosition;
		Camera.main.transform.position = new Vector3 (randomPosition.x, randomPosition.y, Camera.main.transform.position.z);
		Camera.main.GetComponent <FollowPlayer> ().SetOffset();
	}
}
