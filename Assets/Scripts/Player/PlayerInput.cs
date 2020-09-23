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

			if (controller.GetButtonDown ("Dash")) {
				player.OnDashInputDown ();
			}

#if UNITY_EDITOR
			EditorImpulseForceTest ();
#endif
		}

		private void EditorImpulseForceTest () {
			if (controller.GetButtonDown ("LeftClick")) {
				Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				Vector2 direction = (transform.position - mousePosition).normalized;
				player.AddImpulseForce (direction, 15f);
			} else if (controller.GetButtonDown ("RightClick")) {
				Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
				Vector2 direction = (transform.position - mousePosition).normalized;
				player.AddImpulseForce (direction, 60f);
			}
		}
	}
}