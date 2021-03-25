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

	bool displayingControls;
	public Text controlsText;
	public List<PlayerUI> playerUIs = new List<PlayerUI>();

	private void Awake () {
		InitializeSingleton ();
	}

}
