using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public enum PlayerState {
	COMBAT, PUZZLE_BROWSING, PUZZLE_MOVING_SPELLGEM, DEAD
}
public enum PlayerStaffSlot {
	PRIMARY, SECONDARY
}
public class PlayerObject : CreatureObject {

	[SerializeField]
	public Player player;
	public PlayerUI playerUI;

	public WizardGameData wizardGameData;
	private PlayerComponent[] playerComponents;

	//currents.
	public PlayerState currentPlayerState { get; private set; }
	//events (Player)
	public event Action<PlayerState> OnChangePlayerStateEvent;
	public event Action<DashInfo> OnDashEvent;
	//events (Puzzle)
	
	public event Action<PuzzleKey, PuzzleGameData> PickUpStaffEvent;//floor to staff slot (calculated)
	public event Action<PuzzleKey, PuzzleGameData> DropStaffEvent;//staff slot to floor
	public event Action<PuzzleKey, PuzzleGameData> EquipStaffEvent;//set staff as active (in hand)
	public event Action<SpellGemGameData> PickUpSpellGemEvent;
	public event Action<SpellGemGameData> DropSpellGemEvent;
	public event Action<PuzzleGameData, SpellGemGameData> BindSpellGemEvent;
	public event Action<PuzzleGameData, SpellGemGameData> UnbindSpellGemEvent;

	public override void SetupObject () {
		base.SetupObject ();
		playerComponents = GetComponentsInChildren<PlayerComponent> ();
	}
	public override void SubscribeToEvents () {
	}
	public override void UnsubscribeFromEvents () {
	}

	//Called right after PoolObject.Reuse to inject player save data into prefab.
	public void ReusePlayerObject(Player player) {
		this.player = player;
		playerUI = UIManager.Instance.playerUIs [player.playerIndex];
		wizardGameData = WizardGameDataMapper.MapWizardSaveToGameData (player.wizardSaveData);
		OnChangePlayerState (PlayerState.COMBAT); 
		ReusePlayerComponents(player);
	}

	//for every object use
	private void ReusePlayerComponents (Player player) {
		for (int i = 0; i < playerComponents.Length; i++) {
			playerComponents [i].ReusePlayerComponent (player);
		}
	}

	//SETTERS. Changing puzzle values causes events for updating sub-components
	public void OnChangePlayerState (PlayerState playerState) {
		if (currentPlayerState == playerState) {
			return;
		}
		Debug.Log ("PlayerObject new player state. state: " + playerState);
		currentPlayerState = playerState;
		OnChangePlayerStateEvent?.Invoke(playerState);
	}

	//PLAYER
	public override void OnSpawn (Vector3 spawnPosition) {
		base.OnSpawn (spawnPosition);
		OnChangePlayerState (PlayerState.COMBAT);
	}
	public override void OnDeath() {
		base.OnDeath ();
		OnChangePlayerState (PlayerState.DEAD);
		GameManager.instance.ReportPlayerDeath (player);
	}
	public void OnDash (DashInfo dashInfo) {
		OnDashEvent?.Invoke (dashInfo);
	}

	//PUZZLE
	//floor to puzzle region
	public void PickUpStaff (PuzzleKey key, PuzzleGameData puzzleGameData) {
		PickUpStaffEvent?.Invoke (key, puzzleGameData);
		EquipStaff (key, puzzleGameData);
	}

	//puzzle region to floor
	public void DropStaff (PuzzleKey key) {
		if (wizardGameData.puzzleGameDataDictionary.ContainsKey(key)) {
			PuzzleGameData puzzleGameData = wizardGameData.puzzleGameDataDictionary[key];
			DropStaffEvent?.Invoke (key, puzzleGameData);
		}
	}

	//select puzzle region
	public void EquipStaff (PuzzleKey key, PuzzleGameData puzzleGameData) {
		wizardGameData.currentStaffKey = key;
		EquipStaffEvent?.Invoke (key, puzzleGameData);
	}

	//puzzle slot to floor
	public void DropStaff (PuzzleKey key, PuzzleGameData puzzleGameData) {
	DropStaffEvent?.Invoke (key, puzzleGameData);
	}

	//floor to hand
	public void PickUpSpellGem(SpellGemGameData spellGemGameData) {
		PickUpSpellGemEvent?.Invoke (spellGemGameData);
	}
	//hand to floor
	public void DropSpellGem (SpellGemGameData spellGemGameData) {

		DropSpellGemEvent?.Invoke (spellGemGameData);
	}
	//hand to puzzle
	public void BindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGameData) {
		BindSpellGemEvent?.Invoke (puzzleGameData, spellGameData);
		OnChangePlayerState (PlayerState.PUZZLE_BROWSING);
	}

	//puzzle to hand
	public void OnUnbindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGameData) {
		UnbindSpellGemEvent?.Invoke (puzzleGameData, spellGameData);
		OnChangePlayerState (PlayerState.PUZZLE_MOVING_SPELLGEM);
	}

}

