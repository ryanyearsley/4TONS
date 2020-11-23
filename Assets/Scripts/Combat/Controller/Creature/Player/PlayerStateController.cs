using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController : AbstractStateController {

	[SerializeField]

	public Player currentPlayer;
	public PlayerState currentPlayerState { get; private set; }
	public PuzzleSaveDataDictionary currentStaffDictionary;

	public event Action<PlayerState> OnChangeStateEvent;
	public event Action<DashInfo> OnDashEvent;
	public event Action<PuzzleGroupingDetails, SpellSaveData> OnBindSpellGemEvent;
	public event Action<PuzzleGroupingDetails, SpellSaveData> OnUnbindSpellGemEvent;
	public event Action<int, Spell> OnUpdateSpellBindingEvent;
	public event Action<SpellData> OnCastSpellEvent;

	public void InitializeComponent(Player player) {
		currentPlayer = player;
		OnChangeState (PlayerState.COMBAT);
	}

	public void OnChangeState (PlayerState playerState) {
		currentPlayerState = playerState;
		OnChangeStateEvent?.Invoke(playerState);
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
	public void OnCastSpell (SpellData spellData) {
		OnCastSpellEvent?.Invoke (spellData);
	}

}

[SerializeField]
public enum PlayerState {
	COMBAT, PUZZLE_BROWSING, PUZZLE_MOVING_SPELLGEM, DEAD
}