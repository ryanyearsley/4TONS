using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public enum PlayerState {
	COMBAT, PUZZLE_BROWSING, PUZZLE_MOVING_SPELLGEM, DEAD, DISABLED
}
public enum PlayerStaffSlot {
	PRIMARY, SECONDARY
}
public class PlayerObject : CreatureObject {

	[SerializeField]
	public Player player;
	public PlayerUI playerUI;

	public WizardGameData wizardGameData;
	[SerializeField]
	private WizardPrebuildData defaultWizardPrebuildData;
	private PlayerComponent[] playerComponents;

	//currents.
	public PlayerState currentPlayerState { get; private set; }
	//events (Player)
	public event Action<PlayerState> OnChangePlayerStateEvent;

	//events (Combat)
	public event Action<DashInfo> OnDashEvent;
	public event Action<Spell, SpellCastType> CastSpellEvent;
	public event Action<Spell> EndSpellEvent;
	public Spell currentlyCastingSpell;

	//events (Puzzle)

	public event Action<PuzzleKey, PuzzleGameData> PickUpStaffEvent;//floor to staff slot (calculated)
	public event Action<PuzzleKey, PuzzleGameData> DropStaffEvent;//staff slot to floor
	public event Action<PuzzleKey, PuzzleGameData, StaffEquipType> EquipStaffEvent;//set staff as active (in hand)
	public event Action<SpellGemGameData> PickUpSpellGemEvent;
	public event Action<SpellGemGameData> DropSpellGemEvent;
	public event Action<SpellGemGameData, int> RotateSpellGemEvent;
	public event Action<PuzzleGameData, SpellGemGameData, PuzzleBindType> BindSpellGemEvent;
	public event Action<PuzzleGameData, SpellGemGameData, PuzzleUnbindType> UnbindSpellGemEvent;

	public bool usingMouseControls;
	public bool smartCursor;
	public event Action<bool> toggleSmartCursorEvent;
	public AimingMode currentAimingMode;
	public event Action<AimingMode> setAimingModeEvent;

	public SpellGemGameData highlightedSpellGemData;
	public SpellGemGameData movingSpellGemData;

	public override void SetupObject () {
		base.SetupObject ();
		playerComponents = GetComponentsInChildren<PlayerComponent> ();
	}
	public override void SubscribeToEvents () {
		GameManager.instance.gameCompleteEvent += OnGameComplete;
	}
	public override void UnsubscribeFromEvents () {
		GameManager.instance.gameCompleteEvent -= OnGameComplete;
	}

	public void OnGameComplete() {
		ChangePlayerState (PlayerState.DISABLED);
	}
	//Called right after PoolObject.Reuse to inject player save data into prefab.
	public void ReusePlayerObject (Player player) {
		this.player = player;
		playerUI = UIManager.Instance.GetPlayerUIFromPlayerIndex (player.playerIndex);
		if (player.wizardSaveData.spellSchoolData != null) {
			wizardGameData = WizardGameDataMapper.MapWizardSaveToGameData (player.wizardSaveData);
		} else {
			Debug.Log ("PlayerObject: No save data found on player. Cloning defaults.");
			player.wizardSaveData = defaultWizardPrebuildData.wizardSaveData.Clone();
			wizardGameData = WizardGameDataMapper.MapWizardSaveToGameData (player.wizardSaveData);
		}
		ReusePlayerComponents (player);
	}

	//for every object use
	private void ReusePlayerComponents (Player player) {
		for (int i = 0; i < playerComponents.Length; i++) {
			playerComponents [i].ReusePlayerComponent (player);
		}
	}

	//SETTERS. Changing puzzle values causes events for updating sub-components
	public void ChangePlayerState (PlayerState playerState) {
		if (currentPlayerState == playerState) {
			return;
		}
		Debug.Log ("PlayerObject new player state. state: " + playerState);
		currentPlayerState = playerState;
		OnChangePlayerStateEvent?.Invoke (playerState);
	}

	//PLAYER
	public override void OnSpawn (Vector3 spawnPosition) {
		base.OnSpawn (spawnPosition);
		ChangePlayerState (PlayerState.COMBAT);
	}
	public override void OnDeath () {
		base.OnDeath ();
		ChangePlayerState (PlayerState.DEAD);
		GameManager.instance.ReportPlayerDeath (player);
	}

	public void SetUsingMouseControls(bool usingMouseControls) {
		this.usingMouseControls = usingMouseControls;
	}
	public void SetSmartCursor(bool isSmartCursorActive) {
		this.smartCursor = isSmartCursorActive;
		if (!usingMouseControls) {

		}
	}
	public void SetAimingMode(AimingMode aimingMode) {
		currentAimingMode = aimingMode;
		setAimingModeEvent?.Invoke (aimingMode);
	}
	public void OnDash (DashInfo dashInfo) {
		OnDashEvent?.Invoke (dashInfo);
	}
	//COMBAT

	public void OnCastSpell(Spell spell, SpellCastType spellCastType) {
		SpellData spellData = spell.spellData;
		vitalsEntity.resource.SubtractResourceCost (spellData.manaCost);
		CastSpellEvent?.Invoke (spell, spellCastType);
		OnAttack (new AttackInfo (spellData.castTime, spellData.castSpeedReduction, spellData));
		/*
		if (spellCastType == SpellCastType.CAST) {
			OnAttack (new AttackInfo (spellData.castTime, spellData.castSpeedReduction, spellData));
		} else {
			OnAttack (new AttackInfo (spellData.castTime, spellData.castSpeedReduction, spellData));
		}*/
		AddSpeedEffect (new SpeedAlteringEffect (spellData.castSpeedReduction, spellData.castTime, false));
	}
	public void OnEndSpell (Spell spell) {
		if (currentlyCastingSpell == spell) {
			EndSpellEvent?.Invoke (spell);
		}
		currentlyCastingSpell = null;
	}

	//PUZZLE
	//floor to puzzle region
	public void PickUpStaff (PuzzleKey key, PuzzleGameData puzzleGameData) {
		PickUpStaffEvent?.Invoke (key, puzzleGameData);
		EquipStaff (key, puzzleGameData,StaffEquipType.PICK_UP);
	}

	//puzzle region to floor
	public void DropStaff (PuzzleKey key, StaffDropType dropType) {
		if (wizardGameData.puzzleGameDataDictionary.ContainsKey (key)) {
			PuzzleGameData puzzleGameData = wizardGameData.puzzleGameDataDictionary[key];
			DropStaffEvent?.Invoke (key, puzzleGameData);

			if (dropType == StaffDropType.MANUAL_DROP) {
				PuzzleGameData otherStaffPuzzleData = null;
				foreach (KeyValuePair<PuzzleKey, PuzzleGameData> kvp in wizardGameData.puzzleGameDataDictionary) {
					if (kvp.Value.puzzleData.puzzleType != PuzzleType.INVENTORY) {
						otherStaffPuzzleData = kvp.Value;
					}
					// do something with entry.Value or entry.Key
				}

				if (otherStaffPuzzleData != null) {
					Debug.Log ("PlayerPuzzleComponent: Dropping staff. Auto-equipping other weapon.");
					EquipStaff (otherStaffPuzzleData.puzzleKey, otherStaffPuzzleData, StaffEquipType.DROPPED_OTHER);
				} else {
					wizardGameData.currentStaffKey = PuzzleKey.NO_WEAPON;
				}
			} 
		}
	}

	//select puzzle region
	public void EquipStaff (PuzzleKey key, PuzzleGameData puzzleGameData, StaffEquipType equipType) {
		wizardGameData.currentStaffKey = key;
		EquipStaffEvent?.Invoke (key, puzzleGameData, equipType);
	}

	//puzzle slot to floor
	public void DropStaff (PuzzleKey key, PuzzleGameData puzzleGameData, StaffDropType dropType) {
		DropStaffEvent?.Invoke (key, puzzleGameData);
	}

	//floor to hand
	public void PickUpSpellGem (SpellGemGameData spellGemGameData) {
		PickUpSpellGemEvent?.Invoke (spellGemGameData);
	}
	//hand to floor
	public void DropSpellGem (SpellGemGameData spellGemGameData) {

		DropSpellGemEvent?.Invoke (spellGemGameData);
	}
	public void RotateSpellGem (SpellGemGameData spellGemGameData, int rotateIndex) {
		RotateSpellGemEvent?.Invoke (spellGemGameData, rotateIndex);
	}
	//hand to puzzle
	public void BindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGameData, PuzzleBindType bindingType) {
		BindSpellGemEvent?.Invoke (puzzleGameData, spellGameData, bindingType);
		ChangePlayerState (PlayerState.PUZZLE_BROWSING);
	}

	//puzzle to hand
	public void UnbindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGameData, PuzzleUnbindType unbindType) {
		UnbindSpellGemEvent?.Invoke (puzzleGameData, spellGameData, unbindType);
		ChangePlayerState (PlayerState.PUZZLE_MOVING_SPELLGEM);
	}

}

