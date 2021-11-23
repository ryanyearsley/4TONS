using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerCanvasManager : PersistentManager {
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

	protected override void Awake () {
		base.Awake ();
		Debug.Log ("PointerCanvasManager: Awake (hiding/locking cursor)");
		InitializeSingleton ();
		DontDestroyOnLoad (this);
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

}
