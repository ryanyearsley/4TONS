using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	#region Singleton
	public static UIManager Instance { get; private set; }
	private void InitializeSingleton () {
		Instance = this;
	}
	#endregion

	[SerializeField]
	private List<PlayerUI> playerUIs = new List<PlayerUI>();

	public PlayerUI GetPlayerUIFromPlayerIndex(int playerIndex) {
		if (playerUIs.Count > playerIndex) {
			return playerUIs [playerIndex];
		} else return null;
	}
	private void Awake () {
		InitializeSingleton ();
		InitializePlayerUIPanels ();
	}

	private void InitializePlayerUIPanels() {
		int playerCount = 1;	
		if (PlayerManager.instance != null) {
			playerCount = PlayerManager.instance.currentPlayers.Count;
		}
		for (int i = 0; i < playerUIs.Count; i++) {
			if (playerCount > i) {
				playerUIs [i].gameObject.SetActive (true);
			}
			 else {
				playerUIs [i].gameObject.SetActive (false);
			}
		}
	}

}
