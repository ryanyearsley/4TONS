using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpellComponent : PlayerComponent {


	private ResourceComponent manaController;

	private SpellBindingDictionary currentSpellBindingDictionary;

	public override void SetUpComponent (GameObject rootObject) {
		base.SetUpComponent (rootObject);
		manaController = rootObject.GetComponentInChildren<ResourceComponent> ();
	}
	public override void ReusePlayerComponent (Player player) {
		base.ReusePlayerComponent (player);
	}

	public override void OnSpawn (Vector3 spawnPosition) {
		base.OnSpawn (spawnPosition);
		playerObject.SetCanAttack (true);
	}

	public override void OnPickUpStaff (PuzzleKey region, PuzzleGameData puzzleGameData) {
		currentSpellBindingDictionary = puzzleGameData.spellBindingDictionary;
		playerObject.playerUI.OnPickUpStaff (region, puzzleGameData);
	}

	public override void OnEquipStaff (PuzzleKey region, PuzzleGameData puzzleGameData) {
		playerObject.playerUI.OnEquipStaff (region);
		if (currentSpellBindingDictionary != null) {
			foreach (int key in currentSpellBindingDictionary.Keys) {
				if (currentSpellBindingDictionary[key] != null) {
					currentSpellBindingDictionary [key].spellUI = null;//dirty ass un-equip
				}
			}
		}
		currentSpellBindingDictionary = puzzleGameData.spellBindingDictionary;
		for (int i = 0; i < puzzleGameData.spellBindingDictionary.Count; i++) {
			if (puzzleGameData.spellBindingDictionary [i] != null) {
				puzzleGameData.spellBindingDictionary [i].spellUI = playerObject.playerUI.spellUIs [i];
				playerObject.playerUI.spellUIs [i].InitializeSpellUI (puzzleGameData.spellBindingDictionary [i].spellData);
			} else {
				playerObject.playerUI.spellUIs [i].ClearSpellBinding ();
			}
		}

	}

	public override void OnDropStaff (PuzzleKey region, PuzzleGameData puzzleGameData) {
		playerObject.playerUI.OnDropStaff (region);

		if (puzzleGameData.spellBindingDictionary == currentSpellBindingDictionary) {
			currentSpellBindingDictionary = null;
		}

		for (int i = 0; i < playerObject.playerUI.spellUIs.Length; i++) {
			playerObject.playerUI.spellUIs [i].ClearSpellBinding ();
		}
	}
	public override void OnBindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGemGameData) {
		Debug.Log ("PlayerSpell On BindSpellGem");
		if (spellGemGameData.spellCast == null) {
			spellGemGameData.spellCast = Instantiate (spellGemGameData.spellData.castObject).GetComponent<Spell> ();
			spellGemGameData.spellCast.transform.parent = SpellCastObjectManager.instance.transform;
			spellGemGameData.spellCast.transform.localPosition = Vector3.zero;
		}
		if (puzzleGameData.puzzleData.puzzleType != PuzzleType.INVENTORY) {
			puzzleGameData.spellBindingDictionary [spellGemGameData.spellBindIndex] = spellGemGameData.spellCast;
			if (puzzleGameData.puzzleKey == playerObject.wizardGameData.currentStaffKey) {
				spellGemGameData.spellCast.spellUI = UIManager.Instance.playerUIs [playerObject.player.playerIndex].spellUIs [spellGemGameData.spellBindIndex];
				spellGemGameData.spellCast.spellUI.InitializeSpellUI (spellGemGameData.spellData);
			}
		}
		spellGemGameData.spellCast.ConfigureSpellToPlayer (playerObject);
		spellGemGameData.spellCast.tag = this.tag;
	}

	public override void OnUnbindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellSaveData) {
		if (puzzleGameData.puzzleKey == playerObject.wizardGameData.currentStaffKey) {
			spellSaveData.spellCast.spellUI.ClearSpellBinding ();
			spellSaveData.spellCast.spellUI = null;
		}
		puzzleGameData.spellBindingDictionary [spellSaveData.spellBindIndex] = null;
		spellSaveData.spellCast.tag = "Untagged";

		spellSaveData.spellCast.transform.localPosition = Vector3.zero;
	}

	//Used on reuse of a player object, as well as when switching weapons.
	public void OnSpellButtonDown (int spellIndex) {
		if (!playerObject.canAttack || currentSpellBindingDictionary == null || currentSpellBindingDictionary [spellIndex] == null)
			return;
		Spell spell = currentSpellBindingDictionary [spellIndex];
		if (!spell.onCooldown &&
			spell.isCastEligible () &&
			manaController.SubtractResourceCost (spell.spellData.manaCost)) {
			spell.CastSpell ();
			playerObject.OnAttack (new AttackInfo (spell.spellData.castTime, spell.spellData.castSpeedReduction));
			playerObject.AddSpeedEffect (new SpeedAlteringEffect (spell.spellData.castSpeedReduction, spell.spellData.castTime, false));
		}
	}
	public void OnSpellButton (int spellIndex) {
		//channel spell
		if (!!playerObject.canAttack)
			return;
	}
	public void OnSpellButtonUp (int spellIndex) {
		if (!playerObject.canAttack)
			return;
	}
}