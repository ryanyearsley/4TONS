using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialChecklistPanelUI : AbstractPanelUI, ITutorialSubscriber {

	[SerializeField]
	private TMP_Text phaseTitleText;
	[SerializeField]
	private TutorialTasklistElement[] tutorialTaskListElements;

	public override void Start () {
		base.Start ();
		SubscribeToTutorialEvents ();
	}
	public void SubscribeToTutorialEvents () {
		Debug.Log ("TutorialChecklistPanelUI: Subscribing to events");
		TutorialManager.instance.OnStartPhaseEvent += OnStartPhase;
		TutorialManager.instance.OnTaskCompleteEvent += OnTaskComplete;
		TutorialManager.instance.OnPhaseCompleteEvent += OnPhaseComplete;
	}

	public void OnStartPhase (TutorialPhaseInfo tutorialPhaseInfo) {
		Debug.Log ("TutorialCheckListUI: Start Phase");
		phaseTitleText.text = tutorialPhaseInfo.phase.ToString ();
		for (int i = 0; i < tutorialTaskListElements.Length; i++) {
			if (tutorialPhaseInfo.requiredTasks.Length > i) {
				tutorialTaskListElements [i].gameObject.SetActive (true);
				tutorialTaskListElements [i].ReuseTasklistElement (tutorialPhaseInfo.requiredTasks [i]);
			} else {
				tutorialTaskListElements [i].gameObject.SetActive (false);
			}
		}
	}

	public void OnTaskComplete (TutorialTask tutorialTask) {

		Debug.Log ("TutorialCheckListUI: Task complete: " + tutorialTask);
		for (int i = 0; i < tutorialTaskListElements.Length; i++) {
			if (tutorialTaskListElements [i].gameObject.activeInHierarchy) {
				tutorialTaskListElements [i].TrySetTaskComplete (tutorialTask);
			}
		}
	}
	public void OnPhaseComplete (TutorialPhaseInfo completedPhaseInfo) {
		for (int i = 0; i < tutorialTaskListElements.Length; i++) {
			tutorialTaskListElements [i].gameObject.SetActive (false);
		}
	}
}