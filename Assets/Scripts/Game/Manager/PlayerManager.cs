using Rewired;
using System;
using System.Collections.Generic;
using UnityEngine;



public class PlayerManager : MonoBehaviour {
	#region Singleton
	public static PlayerManager Instance { get; private set; }
	private void InitializeSingleton () {
		Instance = this;
	}
	#endregion
	[SerializeField]
	public List<Player> currentPlayers = new List<Player>();

	private void Awake () {
		InitializeSingleton ();
		DontDestroyOnLoad(this.gameObject);
	}
	private void Start () {

	}
	private void Update () {

	}

	public void AddPlayer(Player player) {
		currentPlayers.Add (player);
	}
	public void ConfirmPlayerWizardSelection(int playerIndex, WizardSaveData data) {
		if (currentPlayers.Count >= playerIndex) {
			currentPlayers [playerIndex].currentWizard = data;
			currentPlayers [playerIndex].isReady = true;
		}
	}
} 

[Serializable]
public class Player {
	public int playerIndex;
	public int controllerIndex;
	public bool isReady;
	public WizardSaveData currentWizard;
}