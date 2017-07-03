using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(LivingEntity))]
public class LiveUI : MonoBehaviour {

	public Text liveText;
	LivingEntity livingEntity;
	int currentLiveInText;

	void Awake () {
		livingEntity = GetComponent <LivingEntity> ();
		liveText.text = livingEntity.startLives + "/" + livingEntity.startLives;
		currentLiveInText = livingEntity.startLives;
	}

	void Update () {
		if (currentLiveInText != livingEntity.lives) {
			currentLiveInText = livingEntity.lives;
			liveText.text = livingEntity.startLives + "/" + currentLiveInText;
		}
	}
}
