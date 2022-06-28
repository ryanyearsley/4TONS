using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialVoiceManager : MonoBehaviour
{
	[SerializeField]
	private AudioSource audioSource;

	void Start () {
		TutorialManager.instance.OnBeginTutorialLevel += OnBeginTutorialLevel;
		TutorialManager.instance.OnPhaseCompleteEvent += OnPhaseComplete;
		GameManager.instance.levelCompleteEvent += OnLevelComplete;
	}

	private void OnDisable () {
		TutorialManager.instance.OnBeginTutorialLevel -= OnBeginTutorialLevel;
		TutorialManager.instance.OnPhaseCompleteEvent -= OnPhaseComplete;
		GameManager.instance.levelCompleteEvent -= OnLevelComplete;
	}



	public void OnBeginTutorialLevel(TutorialPhaseInfo tutorialPhaseInfo) {
		if (tutorialPhaseInfo.beginVoiceLine.audioClip != null) {
			audioSource.clip = tutorialPhaseInfo.beginVoiceLine.audioClip;
			audioSource.Play ();
		}
	}

	public void OnPhaseComplete (TutorialPhaseInfo tutorialPhaseInfo) {
		if (tutorialPhaseInfo.completeVoiceLine.audioClip != null) {
			audioSource.clip = tutorialPhaseInfo.completeVoiceLine.audioClip;
			audioSource.Play ();
		}
	}
	public void OnLevelComplete(int floorIndex) {
		audioSource.Stop ();
	}
}
