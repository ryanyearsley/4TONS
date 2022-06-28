using System.Collections;
using UnityEngine;
using TMPro;

public class GameMessagePanelUI : AbstractPanelUI {


	[SerializeField]
	private GameMessageDictionary gameMessageDictionary = new GameMessageDictionary();


	[SerializeField]
	private TMP_Text gameMessageText;


	protected override void OnUIChange(GameState gameState) {
		Debug.Log ("game message ui change");
		if (gameMessageDictionary.ContainsKey (gameState)) {
			DisplayGameMessage (gameMessageDictionary [gameState], 2);
		}
	}

	protected void DisplayGameMessage (string message, float messageDisplayTime) {
		StopAllCoroutines ();
		StartCoroutine (DisplayGameMessageRoutine (message, messageDisplayTime));
	}

	private IEnumerator DisplayGameMessageRoutine (string message, float messageDisplayTime) {
		gameMessageText.text = message;
		yield return new WaitForSeconds (messageDisplayTime);
		gameMessageText.text = "";
	}
}
