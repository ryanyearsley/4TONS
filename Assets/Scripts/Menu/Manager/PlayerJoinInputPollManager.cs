using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

//Polls for player input and calls PlayerJoin Event
public class PlayerJoinInputPollManager : MonoBehaviour {

	public List<Rewired.Player> unassignedControllers = new List<Rewired.Player>();

	private void Awake () {
		PlayerManager playerManager = PlayerManager.instance;
		for (int i = 0; i < 4; i++) {
			Rewired.Player controller = ReInput.players.GetPlayer (i);
			if (playerManager != null && playerManager.currentPlayers.Count > 0) {
				foreach (Player player in playerManager.currentPlayers) {
					if (controller.id != player.controllerIndex) {
						unassignedControllers.Add (ReInput.players.GetPlayer (i));
					}
				}
			} else {
				unassignedControllers.Add (ReInput.players.GetPlayer (i));
			}
		}

	}

	void Update () {
		for (int i = unassignedControllers.Count - 1; i >= 0; i--) {
			if (unassignedControllers [i].GetAnyButtonDown ()) {
				Debug.Log ("Input detected. Player joining.");
				MainMenuManager.Instance.OnPlayerJoin (unassignedControllers [i].id);
				unassignedControllers.RemoveAt (i);
			}
		}
	}


}
