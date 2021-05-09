using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TutorialTask {
	//BASICS 
	MOVEMENT, AIMING, PICK_UP_STAFF_PRIMARY, PUZZLE_TOGGLE, 

	//PUZZLE BIND AUTO
	PICK_UP_SPELLGEM, AUTO_BIND_SPELLGEM, CAST_LEECHBOLT,

	//COMBAT 1 (Zombie)
	KILL_ENEMY,
	//PUZZLE BIND MANUAL
	ROTATE_SPELLGEM, MANUAL_BIND_SPELLGEM, CAST_TENTACLES,

	//Combat 2 (Necro)
	DODGE, 
	
	//Staff transactions
	PICK_UP_STAFF_SECONDARY, MANUAL_EQUIP_PRIMARY, MANUAL_EQUIP_SECONDARY,

	//PUZZLE UNBINDING/DROPPING
	UNBIND_SPELLGEM, DROP_SPELLGEM, DROP_STAFF,

	//ENTER_PORTAL
	ENTER_PORTAL
}

public enum TutorialPhase {
	BASICS, PUZZLE_BIND_AUTO, COMBAT_ZOMBIE, PUZZLE_BIND_MANUAL, COMBAT_NECRO, STAFF_TRANSACTIONS, PUZZLE_DROPPING, ENTER_PORTAL, 
}

[Serializable]
public class TutorialPhaseInfo {
	public TutorialPhase phase;
	public int phaseIndex;
	public TutorialTaskInfo[] requiredTasks;
}

[Serializable]
public class TutorialTaskInfo {
	public TutorialTask task;
	public string input;
	[TextArea(5,4)]
	public string taskDescription;
}

[Serializable]
public class TutorialProgress {


	public int tasksComplete;
	public int totalTasksThisPhase;
	public Dictionary <TutorialTask, bool> currentTaskDictionary = new Dictionary<TutorialTask, bool>();

	public TutorialPhaseInfo currentPhaseInfo;

}
public class TutorialManager : MonoBehaviour {

	public TutorialPhaseInfo[] tutorialPhaseInfos;
	#region Singleton
	public static TutorialManager instance;
	void SingletonInitialization () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion

	//towers are separated by scenes. Hub is also it's own scene.
	[SerializeField]
	private TutorialProgress tutorialProgress;
	private ControlSchemeData currentControlSchemeData;

	public Action<TutorialPhaseInfo> OnStartPhaseEvent;
	public Action<TutorialTask> OnTaskCompleteEvent;
	public Action<TutorialPhaseInfo> OnPhaseCompleteEvent;
	public Action OnTutorialCompleteEvent;

	public Sprite incompleteCheckboxSprite;
	public Sprite completeCheckboxSprite;


	private void Awake () {
		SingletonInitialization ();
	}
	private void Start () {
		InitializeManager ();
	}

	public void UpdateControlScheme (ControlSchemeData controlSchemeData) {
		currentControlSchemeData = controlSchemeData;
	}

	public void InitializeManager () {
		tutorialProgress = new TutorialProgress ();
		GameManager.instance.beginLevelEvent += OnBeginLevel;
	}

	public void OnBeginLevel (int levelIndex) {
		StartPhase (0);
	}

	public void StartPhase(int phaseIndex) {
		TutorialPhaseInfo phaseInfo = tutorialPhaseInfos[phaseIndex];

		Debug.Log ("TutorialManager: Starting Phase " + phaseInfo.phase);
		tutorialProgress.currentPhaseInfo = phaseInfo;
		tutorialProgress.currentPhaseInfo.phase = phaseInfo.phase;
		tutorialProgress.currentPhaseInfo.phaseIndex = phaseIndex;
		tutorialProgress.currentTaskDictionary.Clear ();

		for (int i = 0; i < phaseInfo.requiredTasks.Length; i++) {
			TutorialTaskInfo taskInfo = phaseInfo.requiredTasks[i];
			tutorialProgress.currentTaskDictionary.Add (taskInfo.task, false);
		}
		tutorialProgress.totalTasksThisPhase = tutorialProgress.currentTaskDictionary.Count;
		OnStartPhaseEvent?.Invoke (phaseInfo);
	}

	public bool SetTaskComplete (TutorialTask tutorialTask) {
		Debug.Log ("TutorialManager: Setting task complete!");
		if (tutorialProgress.currentTaskDictionary.ContainsKey (tutorialTask)
			&& tutorialProgress.currentTaskDictionary [tutorialTask] == false) {
			tutorialProgress.currentTaskDictionary [tutorialTask] = true;
			tutorialProgress.tasksComplete++;
			TaskComplete (tutorialTask);
			bool allComplete = true;
			foreach (bool taskComplete in tutorialProgress.currentTaskDictionary.Values) {
				if (!taskComplete) {
					allComplete = false;
				}
			}
			if (allComplete == true) {
				StartCoroutine (PhaseCompleteRoutine ());
			}
			return true;
		} else {
			return false;
		}

	}
	public IEnumerator PhaseCompleteRoutine() {

		yield return new WaitForSeconds (0.5f);
		PhaseComplete (tutorialProgress.currentPhaseInfo);
		int nextPhaseIndex = tutorialProgress.currentPhaseInfo.phaseIndex + 1;
		if (nextPhaseIndex < tutorialPhaseInfos.Length) {
			StartPhase (nextPhaseIndex);
		} else {
			GameManager.instance.GameComplete ();
			SceneManager.LoadScene (0);
		}
	}

	public void TaskComplete (TutorialTask task) {
		Debug.Log ("TutorialManager: Task Complete event: " + task);
		OnTaskCompleteEvent?.Invoke (task);
	}
	public void PhaseComplete (TutorialPhaseInfo phaseInfo) {
		OnPhaseCompleteEvent?.Invoke (phaseInfo);
	}




}
