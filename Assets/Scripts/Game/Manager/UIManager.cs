using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	#region Singleton
	public static UIManager Instance { get; private set; }

	bool displayingControls;
	public Text controlsText;
	private void InitializeSingleton () {
		Instance = this;
	}
	#endregion
	public List<PlayerUI> playerUIs = new List<PlayerUI>();

	private void Awake () {
		InitializeSingleton ();
	}

	private void Update () {
		if (Input.GetKeyDown(KeyCode.C)) {
			if (!displayingControls) {
				displayingControls = true;
				controlsText.gameObject.SetActive (true);
			} else{
				displayingControls = false;
				controlsText.gameObject.SetActive (false);
			}
		}
	}
}
