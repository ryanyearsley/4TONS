using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public enum PlayerState {
	COMBAT, PUZZLE_BROWSING, PUZZLE_MOVING_SPELLGEM, DEAD
}
public class PlayerObject : CreatureObject {

	[SerializeField]
	public Player player;
	PlayerComponent[] playerComponents;

	public PlayerState currentPlayerState { get; private set; }
	public PuzzleSaveDataDictionary currentStaffDictionary;
	public event Action<PlayerState> OnChangePlayerStateEvent;
	public event Action<DashInfo> OnDashEvent;
	public event Action<PuzzleGroupingDetails, SpellSaveData> OnBindSpellGemEvent;
	public event Action<PuzzleGroupingDetails, SpellSaveData> OnUnbindSpellGemEvent;
	public event Action<int, Spell> OnUpdateSpellBindingEvent;


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
		OnChangePlayerState (PlayerState.COMBAT); 
		ReusePlayerComponents(player);
	}

	//for every object use
	private void ReusePlayerComponents (Player player) {
		for (int i = 0; i < playerComponents.Length; i++) {
			playerComponents [i].ReusePlayerComponent (player);
		}
	}

	public void OnChangePlayerState (PlayerState playerState) {
		if (currentPlayerState == playerState) {
			return;
		}
		Debug.Log ("PlayerObject new player state. state: " + playerState);
		currentPlayerState = playerState;
		OnChangePlayerStateEvent?.Invoke(playerState);
	}

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
	public void OnBindSpellGem (PuzzleGroupingDetails details, SpellSaveData spellSaveData) {
		OnBindSpellGemEvent?.Invoke (details, spellSaveData);
	}

	public void OnUnbindSpellGem (PuzzleGroupingDetails details, SpellSaveData spellSaveData) {
		OnUnbindSpellGemEvent?.Invoke (details, spellSaveData);
	}

	public void OnUpdateSpellBinding (int spellIndex, Spell spell) {
		OnUpdateSpellBindingEvent?.Invoke (spellIndex, spell);
	}
}

