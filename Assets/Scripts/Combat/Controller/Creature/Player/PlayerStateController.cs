using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController : AbstractStateController {

	public PlayerPositions playerPositions { get; private set; }
	public PlayerState currentPlayerState { get; private set; }


	public event Action OnDashEvent;


	public void OnDash () {
		OnDashEvent?.Invoke ();
	}

	public void SetPlayerPositions (Transform cursorTransform, Transform feetTransform, Transform staffTransform) {
		playerPositions = new PlayerPositions (cursorTransform, feetTransform, staffTransform);
	}
}

public enum PlayerState {
	COMBAT, PUZZLE, DEAD
}