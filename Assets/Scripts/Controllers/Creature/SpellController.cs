using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellController : MonoBehaviour {

	private PlayerStateController stateController;
	private bool canCast;

	public void OnSpellButton (int spellIndex) {
		if (!canCast)
			return;

		PlayerPositions positions = stateController.playerPositions;
	}

	public void OnSpellButtonDown (int spellIndex) {
		if (!canCast)
			return;

		PlayerPositions positions = stateController.playerPositions;
	}

	public void OnSpellButtonUp (int spellIndex) {
		if (!canCast)
			return;

		PlayerPositions positions = stateController.playerPositions;
	}

	public void SetCanCast (bool canCast) {
		this.canCast = canCast;
	}

	private void OnEnable () {
		stateController = GetComponent<PlayerStateController> ();
		stateController.OnSetCanCastEvent += SetCanCast;
	}

	private void OnDisable () {
		stateController.OnSetCanCastEvent -= SetCanCast;
	}
}