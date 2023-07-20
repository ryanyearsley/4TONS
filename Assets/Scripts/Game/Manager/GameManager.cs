using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages basic game loop and events.
public enum GameState {
	LOADING, COMBAT, PAUSE, GAME_OVER, LEVEL_COMPLETE, GAME_COMPLETE, DECISION
}
public class GameManager : MonoBehaviour {

	#region Singleton
	public static GameManager instance;
	void SingletonInitialization () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion

	//static variables (world data)
	public GameContext gameContext;
	protected GameState currentGameState;

	private static int MAX_LEVEL_LOAD_TIME = 10;

	//game loop
	public event Action<int> loadLevelEvent;//generates map and creates objects.
	public event Action<int> beginLevelEvent;//enables players/enemies/objects
	public event Action<int> levelCompleteEvent;//Portal Open
	public event Action<int> levelEndEvent;//Portal enter, clean up

	//win/loss condition events
	public event Action gameCompleteEvent;//player tower complete
	public event Action<int> gameOverEvent;//player loss event (death)
	
	//other
	public event Action onPauseEvent;//player pause button event

	//Pause stuff
	public bool isPaused = false;
	protected GameState unpauseResumeGameState;
	

	public event Action<GameState> changeGameStateEvent;

	private bool levelLoaded;
	public void SetLevelLoaded () {
		Debug.Log ("level loaded. Begin game!");
		levelLoaded = true;
	}

	private bool levelObjectiveComplete;

	[SerializeField]
	private GameProgress gameProgress;

	public GameProgress GetProgress () {
		return gameProgress;
	}

	private bool isPortalOpen;

	public bool GetLevelLoaded() {
		return levelLoaded;
	}
	void Awake () {
		SingletonInitialization ();
	}
	private void Start () {
		StartCoroutine (BeginGameRoutine ());
	}
	
	private IEnumerator BeginGameRoutine() {
		gameProgress = new GameProgress (gameContext.zoneData.mapDatas.Length - 1);
		//waits for subscribers.
		yield return new WaitForSeconds (0.5f);
		StartCoroutine(LoadLevelRoutine (0));
	}


	private void ChangeGameState (GameState newGameState) { 
		if (currentGameState != newGameState) {
			currentGameState = newGameState;
			changeGameStateEvent?.Invoke (currentGameState);
		}
	}
	public void LoadLevel (int levelIndex) {
		StartCoroutine (LoadLevelRoutine (levelIndex));
	}

	private IEnumerator LoadLevelRoutine (int levelIndex) {
		ChangeGameState (GameState.LOADING);
		Debug.Log ("loading level... ");
		loadLevelEvent?.Invoke (levelIndex);
		levelLoaded = false;
		float waitTime = 0f;
		while (levelLoaded == false) {
			yield return new WaitForSeconds (0.5f);
			waitTime += 0.5f;
			if (waitTime > MAX_LEVEL_LOAD_TIME) {
				Debug.Log ("GameManager: Maximum load time exceeded. Returning to main menu...");
				NERDSTORM.NerdstormSceneManager.instance.LoadMenu ();
			}
			//load level is toggled to true by LevelManager once everything is configured.
			Debug.Log ("waiting for load level...");
		}
		Debug.Log ("level loaded! beginning level");
		BeginLevel (levelIndex);
	}

	public void BeginLevel (int levelIndex) {
		levelObjectiveComplete = false;
		ChangeGameState (GameState.COMBAT);
		beginLevelEvent?.Invoke (levelIndex);
	}

	public void GameOver (int levelLost) {
		ChangeGameState (GameState.GAME_OVER);
		gameOverEvent?.Invoke (levelLost);
	}
	public void ResumeCombat() {
		if (currentGameState != GameState.COMBAT)
			ChangeGameState (GameState.COMBAT);
	}
	public void MakeDecision (int level) {
		ChangeGameState (GameState.DECISION);
	}

	public void LevelObjectiveComplete () {
		levelObjectiveComplete = true;
		ChangeGameState (GameState.LEVEL_COMPLETE);
		levelCompleteEvent?.Invoke (gameProgress.currentLevelIndex);
	}
	public void LevelEnd (int levelEnded) {
		ChangeGameState (GameState.LOADING);
		levelEndEvent?.Invoke (levelEnded);
	}
	public void GameComplete () {
		ChangeGameState (GameState.GAME_COMPLETE);
		gameCompleteEvent?.Invoke ();
	}

	public void LevelEnd () {
		if (levelObjectiveComplete) {
			StartCoroutine (PortalEntryRoutine ());
		}
	}

	public IEnumerator PortalEntryRoutine () {
		levelObjectiveComplete = false;
		Debug.Log ("GameManager: Level " + gameProgress.currentLevelIndex + " complete. ");
		int nextLevelIndex = gameProgress.currentLevelIndex + 1;
		//time for portal entry animation...
		yield return new WaitForSeconds (0.5f);
		if (nextLevelIndex <= gameProgress.finalLevelIndex) {
			GameManager.instance.LevelEnd (gameProgress.currentLevelIndex);
			gameProgress.currentLevelIndex = nextLevelIndex;
			yield return new WaitForSeconds (0.5f);
			GameManager.instance.LoadLevel (gameProgress.currentLevelIndex);
		} else {
			//final level completed. return to hub.
			Debug.Log ("GameManager: Game complete. ");
			GameManager.instance.LevelEnd (gameProgress.currentLevelIndex);
			GameManager.instance.GameComplete ();
			yield break;
		}
		//time for loading screen....
		yield return new WaitForSeconds (1f);
	}
	public void ReportPlayerDeath (Player player) {
		player.isAlive = false;
		player.currentPlayerObject = null;

		bool gameOver = true;
		foreach (Player currentPlayer in PlayerManager.instance.currentPlayers) {
			if (currentPlayer.isAlive)
				gameOver = false;
		}
		if (gameOver) {
			gameOverEvent?.Invoke (1);
			changeGameStateEvent?.Invoke (GameState.GAME_OVER); 
			foreach (Player currentPlayer in PlayerManager.instance.currentPlayers)
			{
				WizardSaveData wizardSaveData = WizardSaveDataMapper.MapGameToSaveData(currentPlayer.currentPlayerObject.wizardGameData);
				WizardSaveDataManager.instance.SaveDeadWizard(wizardSaveData);
			}
		}
	}


	public void OnPause() {
		if (!isPaused) {
			Time.timeScale = 0;
			unpauseResumeGameState = currentGameState;
			changeGameStateEvent?.Invoke (GameState.PAUSE);
			isPaused = true;
		} else {
			Time.timeScale = 1;
			changeGameStateEvent?.Invoke (unpauseResumeGameState);
			isPaused = false;
		}
	}

}