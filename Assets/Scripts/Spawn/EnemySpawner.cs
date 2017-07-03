using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

	[Header("Spawn config")]
	public int count;

	[Header("Enemy config")]
	public GameObject enemyPrefab;
	Transform player;
	public float walkSpeed;
	public float runSpeed;
	public float linearDrag;
	public float projectyleSpeed;
	public float shootDelay;
	public bool smartShooting;

	GameObject[] spawnedEnemies;

	void Awake () {
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent <Transform> ();
	}

	public void Start () {
		StartCoroutine (CreateEnemyesInRandomPlaces ());	
	}

	IEnumerator CreateEnemyesInRandomPlaces () {
		for (int i = 0; i < count; i++) {
			Vector3 randomPosition = Grid.GetRandomWalkablePoint ();

			GameObject enemy = Instantiate (enemyPrefab, randomPosition, Quaternion.identity) as GameObject;
			enemy = AssignEnemy (enemy);
			enemy.name = "Enemy " + (i + 1);
			yield return null;
		}
	}

	GameObject AssignEnemy (GameObject enemy) {
		enemy.GetComponent <EnemyMoveController> ().walkSpeed = walkSpeed;
		enemy.GetComponent <EnemyMoveController> ().runSpeed = runSpeed;
		enemy.GetComponent <Rigidbody2D> ().drag = linearDrag;
		enemy.GetComponent <EnemyMoveController> ().player = player;
		enemy.GetComponent <ShootingEntety> ().projectyleSpeed = projectyleSpeed;
		enemy.GetComponent <ShootingEntety> ().shootDelay = shootDelay;
		enemy.GetComponent <EnemyShootController> ().smartShooting = smartShooting;
		enemy.GetComponent <EnemyShootController> ().targetMovingEntity = player.GetComponent <MovingEntety> ();
		return enemy;
	}

}
