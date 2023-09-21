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

	private PlayerUI playerUI;

	public override void Start () {
		base.Start ();
		SubscribeToTutorialEvents ();
		playerUI = UIManager.Instance.GetPlayerUIFromPlayerIndex(0);
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
		tutorialTaskListElement.ReuseTasklistElement (tutorialPhaseInfo.requiredTasks [0], GetBindingString(tutorialPhaseInfo.requiredTasks[0].input));
	}

	private string GetBindingString (string[] inputs)
	{
		string binding = "";
		foreach (string bindingString in inputs)
		{
			if (binding != string.Empty)
			{
				binding += ", ";
			}
			binding += playerUI.GetCurrentBinding(bindingString);
		}
		return binding;
	}

	public void OnTaskComplete (TutorialTask tutorialTask) {
		StartCoroutine (UpdateTaskRoutine (tutorialTask));
	}

	public IEnumerator UpdateTaskRoutine(TutorialTask tutorialTask) {
		tutorialTaskListElement.TrySetTaskComplete (tutorialTask);
		yield return new WaitForSeconds (1f);
		if (currentPhaseInfo.tasksComplete < currentPhaseInfo.requiredTasks.Length) {

			tutorialTaskListElement.ReuseTasklistElement (currentPhaseInfo.requiredTasks [currentPhaseInfo.tasksComplete], GetBindingString(currentPhaseInfo.requiredTasks[currentPhaseInfo.tasksComplete].input));
		} else
		{
			tutorialTaskListElement.SetTutorialLevelComplete();
		}
	}
	public void OnPhaseComplete (TutorialPhaseInfo completedPhaseInfo) {
		tutorialTaskListElement.gameObject.SetActive (false);
	}
}
