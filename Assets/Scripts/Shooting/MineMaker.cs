using UnityEngine;
using System.Collections;

public class MineMaker : MonoBehaviour {

	public GameObject minePrefab;

	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			Instantiate (minePrefab, transform.position, Quaternion.identity);
		}
	}
}
