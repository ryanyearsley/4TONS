using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
public class PlayerMenuCursorController : MonoBehaviour {
	[SerializeField]
	private int playerNumber;
	private Camera camera;
	private Rewired.Player controller;

	private Transform cursorTransform;
	private bool usingMouseControls;
	//UI
	private SpriteRenderer spriteRenderer;
	public void InitializeMenuCursor (Player player) {
		camera = Camera.main;
		cursorTransform = this.transform;
		controller = ReInput.players.GetPlayer (player.controllerIndex);
		playerNumber = player.playerIndex + 1;
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	private void Update () {
		//move cursor around;
	}
	private void CursorUpdateMouse () {
		Vector2 mouseDelta = new Vector2 (controller.GetAxis ("MouseX"), controller.GetAxis ("MouseY"));
		if (mouseDelta != Vector2.zero) {
			usingMouseControls = true;
			cursorTransform.parent = camera.transform;
		}

		if (usingMouseControls) {
			MouseCursorMovement (mouseDelta);
		}
	}
	private void MouseCursorMovement (Vector2 mouseDelta) {
		Bounds screenBounds = camera.OrthographicBounds ();
		float x = Mathf.Clamp (cursorTransform.position.x + mouseDelta.x * 0.0125f, screenBounds.min.x, screenBounds.max.x);
		float y = Mathf.Clamp (cursorTransform.position.y + mouseDelta.y * 0.0125f, screenBounds.min.y, screenBounds.max.y);
		cursorTransform.position = new Vector3 (x, y, -9f);
	}
}
