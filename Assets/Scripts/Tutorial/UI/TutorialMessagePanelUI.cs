using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMessagePanelUI : GameMessagePanelUI {
	protected override void InitializePanel () {
		GameManager.instance.gameCompleteEvent += OnGameComplete;
		base.InitializePanel ();
	}

	public void OnGameComplete () {
		DisplayGameMessage ("Tutorial Complete! \n Returning to menu...", 3f);
	}
}
