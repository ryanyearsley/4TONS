using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController : AbstractStateController {

	public PlayerState currentPlayerState { get; private set; }


	public event Action OnDashEvent;



	public void OnDash () {
		OnDashEvent?.Invoke ();
	}

}

public enum PlayerState {
	COMBAT, PUZZLE, DEAD
}