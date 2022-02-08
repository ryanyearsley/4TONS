using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NERDSTORM;

public enum TutorialTask {
	//FLOOR 1
	MOVEMENT, AIMING, PICK_UP_STAFF_PRIMARY, DODGE,

	//FLOOR 2
	PUZZLE_TOGGLE, PICK_UP_LEECHBOLT, BIND_LEECHBOLT, CAST_LEECHBOLT,

	//FLOOR 3
	KILL_ENEMY,	ROTATE_SPELLGEM, BIND_TENTACLES, CAST_TENTACLES,

	//FLOOR 4
	UNBIND_TENTACLES, BIND_TENTACLE_INVENTORY, MANUALBIND_HARVEST, CAST_HARVEST,

	//FLOOR 5
	PICK_UP_STAFF_SECONDARY, MANUAL_EQUIP_PRIMARY, MANUAL_EQUIP_SECONDARY,
	
	//FLOOR 6
	DROP_SPELLGEM, DROP_STAFF

}

public enum TutorialPhase {
	Basics, Puzzled, Contemplation, Salvage, Sacrifice 
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

	private ZoneData currentZoneData;
	[SerializeField]
	private TutorialProgress tutorialProgress;
	private ControlSchemeData currentControlSchemeData;

	public Action<TutorialPhaseInfo> OnBeginTutorialLevel;
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
		currentZoneData = GameManager.instance.gameContext.zoneData;
		tutorialProgress = new TutorialProgress ();
		GameManager.instance.beginLevelEvent += OnBeginLevel;
		GameManager.instance.gameCompleteEvent += OnGameComplete;
	}

	public void OnBeginLevel (int levelIndex) {
		BeginTutorialLevel (levelIndex);
		//PlayerManager.instance.currentPlayers [0].currentPlayerObject.gameObject.AddComponent<PlayerTutorialComponent> ();
	}

	public void BeginTutorialLevel(int phaseIndex) {
		MapData currentMapData = currentZoneData.mapDatas[phaseIndex];
		if (currentMapData is TutorialMapData) {
			TutorialMapData currentTutorialMapData = (TutorialMapData) currentMapData;
			TutorialPhaseInfo phaseInfo = currentTutorialMapData.tutorialPhaseInfo;
			tutorialProgress.currentPhaseInfo = phaseInfo;
			Debug.Log ("TutorialManager: Starting Phase " + phaseInfo.phaseIndex);
			tutorialProgress.currentPhaseInfo.phase = phaseInfo.phase;
			tutorialProgress.currentPhaseInfo.phaseIndex = phaseIndex;
			tutorialProgress.currentTaskDictionary.Clear ();

			for (int i = 0; i < phaseInfo.requiredTasks.Length; i++) {
				TutorialTaskInfo taskInfo = phaseInfo.requiredTasks[i];
				tutorialProgress.currentTaskDictionary.Add (taskInfo.task, false);
			}
			tutorialProgress.totalTasksThisPhase = tutorialProgress.currentTaskDictionary.Count;
			OnBeginTutorialLevel?.Invoke (phaseInfo);
		} else {
			Debug.LogError ("Manager/Map Data Mismatch (Configuration Error)");
		}
	}

	public bool SetTaskComplete (TutorialTask tutorialTask) {
		Debug.Log ("TutorialManager: Setting task complete!");
		if (tutorialProgress.currentTaskDictionary.ContainsKey (tutorialTask)
			&& tutorialProgress.currentTaskDictionary [tutorialTask] == false) {
			tutorialProgress.currentTaskDictionary [tutorialTask] = true;
			tutorialProgress.tasksComplete++;
			OnTaskCompleteEvent?.Invoke (tutorialTask);
			bool allInPhaseComplete = true;
			foreach (bool taskComplete in tutorialProgress.currentTaskDictionary.Values) {
				if (!taskComplete) {
					allInPhaseComplete = false;
				}
			}
			if (allInPhaseComplete == true) {
				GameManager.instance.LevelObjectiveComplete ();
			}
			return true;
		} else {
			return false;
		}
	}

	public void TaskComplete (TutorialTask task) {
		OnTaskCompleteEvent?.Invoke (task);
	}
	public void PhaseComplete (TutorialPhaseInfo phaseInfo) {
		OnPhaseCompleteEvent?.Invoke (phaseInfo);
	}

	public void OnGameComplete() {
		Debug.Log ("TutorialManager: Tutorial finished, returning to menu...");
		NerdstormSceneManager.instance.LoadMenu ();
	}



}
