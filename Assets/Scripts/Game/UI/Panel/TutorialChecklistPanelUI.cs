using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialChecklistPanelUI : AbstractPanelUI, ITutorialSubscriber {

	[SerializeField]
	private TMP_Text phaseTitleText;
	[SerializeField]
	private TutorialTasklistElement tutorialTaskListElement;

	private TutorialPhaseInfo currentPhaseInfo;

	public override void Start () {
		base.Start ();
		SubscribeToTutorialEvents ();
	}
	public void SubscribeToTutorialEvents () {
		TutorialManager.instance.OnBeginTutorialLevel += OnStartPhase;
		TutorialManager.instance.OnTaskCompleteEvent += OnTaskComplete;
		TutorialManager.instance.OnPhaseCompleteEvent += OnPhaseComplete;
	}

	public void OnStartPhase (TutorialPhaseInfo tutorialPhaseInfo) {
		currentPhaseInfo = tutorialPhaseInfo;
		phaseTitleText.text = tutorialPhaseInfo.phase.ToString ();
		tutorialTaskListElement.gameObject.SetActive (true);
		tutorialTaskListElement.ReuseTasklistElement (tutorialPhaseInfo.requiredTasks [0]);
	}

	public void OnTaskComplete (TutorialTask tutorialTask) {
		StartCoroutine (UpdateTaskRoutine (tutorialTask));
	}

	public IEnumerator UpdateTaskRoutine(TutorialTask tutorialTask) {
		tutorialTaskListElement.TrySetTaskComplete (tutorialTask);
		yield return new WaitForSeconds (1f);
		if (currentPhaseInfo.tasksComplete < currentPhaseInfo.requiredTasks.Length) {
			tutorialTaskListElement.ReuseTasklistElement (currentPhaseInfo.requiredTasks [currentPhaseInfo.tasksComplete]);
		} else
		{
			tutorialTaskListElement.SetTutorialLevelComplete();
		}
	}
	public void OnPhaseComplete (TutorialPhaseInfo completedPhaseInfo) {
		tutorialTaskListElement.gameObject.SetActive (false);
	}
}
