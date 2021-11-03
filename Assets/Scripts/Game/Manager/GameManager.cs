using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages basic game loop and events.
public enum GameState {
	LOADING, COMBAT, PAUSE, GAME_OVER, LEVEL_COMPLETE, GAME_COMPLETE
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
	

	public event Action<GameState> UIChangeEvent;

	private bool levelLoaded;
	public void SetLevelLoaded () {
		Debug.Log ("level loaded. Begin game!");
		levelLoaded = true;
	}

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
		//waits for subscribers.
		yield return new WaitForSeconds (0.5f);
		StartCoroutine(LoadLevelRoutine (0));
	}


	private void ChangeGameState (GameState newGameState) { 
		if (currentGameState != newGameState) {
			currentGameState = newGameState;
			UIChangeEvent?.Invoke (currentGameState);
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
			yield return new WaitForSeconds (0.1f);
			waitTime += 0.1f;
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
		Debug.Log ("Begin Level");
		ChangeGameState (GameState.COMBAT);
		beginLevelEvent?.Invoke (levelIndex);
	}

	public void GameOver (int levelLost) {
		ChangeGameState (GameState.GAME_OVER);
		gameOverEvent?.Invoke (levelLost);
	}

	public void LevelObjectiveComplete (int levelComplete) {
		ChangeGameState (GameState.LEVEL_COMPLETE);
		levelCompleteEvent?.Invoke (levelComplete);
	}
	public void LevelEnd (int levelEnded) {
		ChangeGameState (GameState.LOADING);
		levelEndEvent?.Invoke (levelEnded);
	}
	public void GameComplete () {
		ChangeGameState (GameState.GAME_COMPLETE);
		gameCompleteEvent?.Invoke ();
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
			UIChangeEvent?.Invoke (GameState.GAME_OVER);
		}
	}


	public void OnPause() {
		if (!isPaused) {
			Time.timeScale = 0;
			unpauseResumeGameState = currentGameState;
			UIChangeEvent?.Invoke (GameState.PAUSE);
			isPaused = true;
		} else {
			Time.timeScale = 1;
			UIChangeEvent?.Invoke (unpauseResumeGameState);
			isPaused = false;
		}
	}

}