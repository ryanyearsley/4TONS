using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioComponent : PlayerComponent
{
	public override void OnChangePlayerState (PlayerState playerState) {
		switch (playerState) {
			case (PlayerState.COMBAT): {
					AudioManager.instance.PlaySound ("TogglePuzzleOff");
					break;
				}
			case (PlayerState.PUZZLE_BROWSING): {
					AudioManager.instance.PlaySound ("TogglePuzzleOn");
					break;
				}
		}
	}
	public override void OnDash (DashInfo dashInfo) {
		AudioManager.instance.PlaySound ("RollDodge");
	}
	public override void OnCastSpell (Spell spell, SpellCastType spellCastType) {
		if (spell.spellData.spellCastSound != null)
			AudioManager.instance.PlaySound (spell.spellData.spellCastSound.clipName);
	}
	public override void OnPickUpStaff (PuzzleKey region, PuzzleGameData puzzleGameData) {
		AudioManager.instance.PlaySound ("StaffPickUp");
	}
	public override void OnDropStaff (PuzzleKey region, PuzzleGameData puzzleGameData) {
		AudioManager.instance.PlaySound ("StaffDrop");
	}
	public override void OnEquipStaff (PuzzleKey region, PuzzleGameData puzzleGameData, StaffEquipType equipType) {
		if (equipType == StaffEquipType.MANUAL_SWAP) {
			AudioManager.instance.PlaySound ("StaffSwitch");
		}
	}
	public override void OnPickUpSpellGem (SpellGemGameData spellGemGameData) {
		AudioManager.instance.PlaySound ("GemPickUp");
	}
	public override void OnDropSpellGem (SpellGemGameData spellGemGameData) {
		AudioManager.instance.PlaySound ("GemDrop");
	}
	public override void OnRotateSpellGem (SpellGemGameData spellGemGameData, int rotateIndex) {
		AudioManager.instance.PlaySound ("GemRotate");
	}
	public override void OnBindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGameData, PuzzleBindType bindType) {
		AudioManager.instance.PlaySound ("GemBind");
	}
	public override void OnUnbindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGameData, PuzzleUnbindType unbindType) {
		AudioManager.instance.PlaySound ("GemUnbind");
	}
}
