using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

	bool isInPause;
	public GameObject restartButton;

	void Awake () {
		isInPause = false;
		restartButton.SetActive (false);
	}

	public void Pause () {
		if (!isInPause) {
			isInPause = true;
			restartButton.SetActive (true);
			Time.timeScale = 0f;
		} else {
			isInPause = false;
			restartButton.SetActive (false);
			Time.timeScale = 1f;
		}
	}

	public void Restart () {
		Time.timeScale = 1f;
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}
}
