using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NERDSTORM;

public enum TutorialTask {
	//FLOOR 1
	MOVEMENT, AIMING, DODGE,

	//FLOOR 2
	PICK_UP_STAFF_PRIMARY, PUZZLE_TOGGLE_ON, PUZZLE_TOGGLE_OFF,

	//FLOOR 3
	PICK_UP_SPELLGEM, BIND_LEECHBOLT, CAST_LEECHBOLT,

	//FLOOR 3
	KILL_ENEMY,	ROTATE_SPELLGEM, PICK_UP_TENTACLES, MANUALBIND_TENTACLES, CAST_TENTACLES,

	//FLOOR 4
	UNBIND_SPELLGEM, BIND_TENTACLE_INVENTORY, MANUALBIND_HARVEST, CAST_HARVEST,

	//FLOOR 5
	PICK_UP_STAFF_SECONDARY, MANUAL_EQUIP_PRIMARY, MANUAL_EQUIP_SECONDARY,
	
	//FLOOR 6
	DROP_SPELLGEM, DROP_STAFF,

	//LATE ADDITIONS
	QUICK_SWAP_STAFF, QUICK_SWAP_SPELLGEMS

}

public enum TutorialPhase {
	Basics, Puzzled, Contemplation, Salvage, Sacrifice 
}

[Serializable]
public class TutorialPhaseInfo {

	//static
	public TutorialPhase phase;
	public int phaseIndex;
	public TutorialTaskInfo[] requiredTasks;
	public VoiceLine beginVoiceLine;
	public VoiceLine completeVoiceLine;

	//dynamic
	public int tasksComplete;
	public int totalTasksThisPhase;
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


	public TutorialTask currentTask;
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
	public Action <TutorialTaskInfo> UpdateTaskEvent;
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
			tutorialProgress.currentPhaseInfo.tasksComplete = 0;
			tutorialProgress.currentTask = tutorialProgress.currentPhaseInfo.requiredTasks [tutorialProgress.currentPhaseInfo.tasksComplete].task;
			tutorialProgress.currentPhaseInfo.totalTasksThisPhase = tutorialProgress.currentPhaseInfo.requiredTasks.Length;
			OnBeginTutorialLevel?.Invoke (phaseInfo);
		} else {
			Debug.LogError ("Manager/Map Data Mismatch (Configuration Error)");
		}
	}
	

	public bool SetTaskComplete (TutorialTask tutorialTask) {
		Debug.Log ("TutorialManager: Setting task complete!");
		if (tutorialProgress.currentTask == tutorialTask) {
			tutorialProgress.currentPhaseInfo.tasksComplete++;
			OnTaskCompleteEvent?.Invoke (tutorialTask);
			Debug.Log ("tasks complete: " + tutorialProgress.currentPhaseInfo.tasksComplete  + ", tasks this phase: " + tutorialProgress.currentPhaseInfo.totalTasksThisPhase);
			if (tutorialProgress.currentPhaseInfo.totalTasksThisPhase > tutorialProgress.currentPhaseInfo.tasksComplete) {
				tutorialProgress.currentTask = tutorialProgress.currentPhaseInfo.requiredTasks [tutorialProgress.currentPhaseInfo.tasksComplete].task;
			} else {
				GameManager.instance.LevelObjectiveComplete ();
			}
			return true;
		} else {
			return false;
		}
	}

	public void UpdateTask(TutorialTaskInfo taskInfo) {

		UpdateTaskEvent?.Invoke (taskInfo);
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
