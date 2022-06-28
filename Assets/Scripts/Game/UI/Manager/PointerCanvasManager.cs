using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired.Components;
using UnityEngine.SceneManagement;

public class PointerCanvasManager : PersistentManager {
	#region Singleton
	public static PointerCanvasManager instance { get; private set; }
	protected override void InitializeSingleton () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion
	[SerializeField]
	private List<PlayerMouse> playerMice = new List<PlayerMouse>();

	protected override void Awake () {
		base.Awake ();
		Debug.Log ("PointerCanvasManager: Awake (hiding/locking cursor)");
		InitializeSingleton ();
		DontDestroyOnLoad (this);
	}

	public override void InitializePersistentManager () {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		ActivatePlayerMice ();
	}

	public override void SceneLoaded (Scene scene, LoadSceneMode loadSceneMode) {
		base.SceneLoaded (scene, loadSceneMode);
		ActivatePlayerMice ();
	}

	private void ActivatePlayerMice() {
		int currentPlayerCount = PlayerManager.instance.currentPlayers.Count;
		Debug.Log ("PointerCanvasManager: Activating " + currentPlayerCount + " Player Mice");
		for (int i = 0; i < playerMice.Count; i++) {
			if (i < currentPlayerCount) {
				playerMice [i].gameObject.SetActive (true);
			} else {
				playerMice [i].gameObject.SetActive (false);
			}
		}
	}

	public override void SubscribeToEvents () {
		base.SubscribeToEvents ();
		if (MainMenuManager.Instance != null) {
			MainMenuManager.Instance.OnPlayerJoinEvent += OnPlayerJoin;
		}
	}
	public override void UnsubscribeFromEvents () {
		base.UnsubscribeFromEvents ();
	}

	public void OnPlayerJoin (Player player) {
		Debug.Log ("PointerCanvasManager: Player " + player.playerIndex + 1 + " joined.");
		if (!playerMice[player.playerIndex].gameObject.activeInHierarchy) {
			playerMice [player.playerIndex].gameObject.SetActive(true);
		}
	}

	private void DisableAllMice() {
		foreach (PlayerMouse mouse in playerMice) {
			mouse.gameObject.SetActive (false);
		}
	}

}
