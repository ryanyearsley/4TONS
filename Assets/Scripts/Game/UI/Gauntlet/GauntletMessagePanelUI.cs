using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GauntletMessagePanelUI : GameMessagePanelUI { 
	protected override void InitializePanel () {
		GameManager.instance.loadLevelEvent += OnLoadLevel;
		GameManager.instance.levelCompleteEvent += OnLevelComplete;
		GameManager.instance.gameCompleteEvent += OnGameComplete;
		base.InitializePanel ();
	}

	public void OnLevelComplete (int currentFloor) {
		DisplayGameMessage ("All enemies killed!", 2f);
	}
	public void OnLoadLevel (int nextFloor) {
		DisplayGameMessage ("Ascending...", 1f);
	}
	public void OnGameComplete () {
		DisplayGameMessage ("Tower Complete!", 3f);
	}
}
