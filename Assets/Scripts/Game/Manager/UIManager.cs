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
	}

}
