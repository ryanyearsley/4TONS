using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDoorComponent : MonoBehaviour, ITutorialSubscriber {

	[SerializeField]
	public Collider2D doorCollider;

	[SerializeField]
	private bool opened;

	private Animator animator;

	public TutorialPhase doorOpenPhase;
	 void Start () {
		SubscribeToTutorialEvents ();
		animator = GetComponentInChildren<Animator> ();
		CloseDoor ();

	}
	public void SubscribeToTutorialEvents () {
		Debug.Log ("TutorialChecklistPanelUI: Subscribing to events");
		TutorialManager.instance.OnStartPhaseEvent += OnStartPhase;
		TutorialManager.instance.OnTaskCompleteEvent += OnTaskComplete;
		TutorialManager.instance.OnPhaseCompleteEvent += OnPhaseComplete;
	}

	public void OnStartPhase (TutorialPhaseInfo tutorialPhaseInfo) {
	}

	public void OnTaskComplete (TutorialTask task) {
	}
	public void OnPhaseComplete (TutorialPhaseInfo completedPhaseInfo) {
		if (completedPhaseInfo.phase == doorOpenPhase) {
			OpenDoor ();
		}
	}

	public void OpenDoor() {
		animator.SetTrigger ("open");
		doorCollider.enabled = false;
		opened = true;
	}

	public void CloseDoor () {
		animator.SetTrigger ("close");
		doorCollider.enabled = true;
		opened = false;
	}

}
