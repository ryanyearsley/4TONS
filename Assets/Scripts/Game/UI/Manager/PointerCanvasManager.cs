using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerCanvasManager : MonoBehaviour {
	#region Singleton
	public static PointerCanvasManager instance { get; private set; }
	private void InitializeSingleton () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion

	[SerializeField]
	private List<Rewired.Components.PlayerMouse> playerMice = new List<Rewired.Components.PlayerMouse>();


	private void Awake () {
		InitializeSingleton ();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

	}

	void Start () {/*
		int activePlayers = PlayerManager.instance.currentPlayers.Count;
		for (int i = 0; i < playerMice.Count; i++) {
			playerMice [i].gameObject.SetActive (true);
		}*/
	}
}
