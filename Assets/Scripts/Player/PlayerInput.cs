using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

namespace PlayerManagement {
	[RequireComponent (typeof (Player))]
	public class PlayerInput : MonoBehaviour {

		private Player player;
		private Rewired.Player controller;

		private void Start () {
			player = GetComponent<Player> ();

			
			controller = ReInput.players.GetPlayer (0);
		}

		private void Update () {
			Vector2 directionalInput = new Vector2 (controller.GetAxisRaw ("MoveHorizontal"), controller.GetAxisRaw ("MoveVertical"));
			player.SetDirectionalInput (directionalInput);
		}
	}
}