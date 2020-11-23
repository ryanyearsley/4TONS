using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	#region Singleton
	public static UIManager Instance { get; private set; }
	private void InitializeSingleton () {
		Instance = this;
	}
	#endregion
	public List<PlayerUI> playerUIs = new List<PlayerUI>();

	private void Awake () {
		InitializeSingleton ();
	}
}
