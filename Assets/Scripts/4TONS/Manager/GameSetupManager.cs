using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetupManager : PersistentManager {
	#region Singleton
	public static GameSetupManager instance { get; private set; }
	private void InitializeSingleton () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion
	GameContext currentGameContext;

	private Objective selectedObjective;

	protected override void Awake () {
		base.Awake ();
		InitializeSingleton ();
	}

}
