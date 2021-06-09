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
		LoadSpellCastEntites (playerObject.wizardGameData);
		playerObject.SetCanAttack (true);
	}
	public void LoadSpellCastEntites (WizardGameData wizardGameData) {
		if (wizardGameData.puzzleGameDataDictionary.ContainsKey (PuzzleKey.INVENTORY)) {
			PuzzleGameData inventoryGameData = wizardGameData.puzzleGameDataDictionary[PuzzleKey.INVENTORY];
			Debug.Log ("PuzzleComponent: Creating Staff PuzzleEntity.");
			LoadSpellCast (inventoryGameData);
		}
		if (wizardGameData.puzzleGameDataDictionary.ContainsKey (PuzzleKey.PRIMARY_STAFF)) {
			PuzzleGameData primaryStaffGameData = wizardGameData.puzzleGameDataDictionary[PuzzleKey.PRIMARY_STAFF];
			LoadSpellCast (primaryStaffGameData);
		}

		if (wizardGameData.puzzleGameDataDictionary.ContainsKey (PuzzleKey.SECONDARY_STAFF)) {
			PuzzleGameData secondaryStaffGameData = wizardGameData.puzzleGameDataDictionary[PuzzleKey.SECONDARY_STAFF];
			LoadSpellCast (secondaryStaffGameData);
		}

	}

	private void LoadSpellCast (PuzzleGameData puzzleGameData) {
		foreach (SpellGemGameData spellGemGameData in puzzleGameData.spellGemGameDataDictionary.Values) {
			OnBindSpellGem (puzzleGameData, spellGemGameData, PuzzleBindType.MANUAL);
		}
	}

	public override void OnPickUpStaff (PuzzleKey region, PuzzleGameData puzzleGameData) {
		playerObject.playerUI.OnPickUpStaff (region, puzzleGameData);
		foreach (SpellGemGameData spellGemGameData in puzzleGameData.spellGemGameDataDictionary.Values) {
			spellGemGameData.spellCast.ConfigureSpellToPlayer (playerObject);
		}
	}

	public override void OnEquipStaff (PuzzleKey region, PuzzleGameData puzzleGameData, StaffEquipType equipType) {
		playerObject.playerUI.OnEquipStaff (region);
		if (currentSpellBindingDictionary != null) {
			for (int i = 0; i < currentSpellBindingDictionary.Count; i++) {
				if (currentSpellBindingDictionary [i] != null) {
					currentSpellBindingDictionary [i].spellUI = null;
				}
			}
		}

		currentSpellBindingDictionary = puzzleGameData.spellBindingDictionary;
		for (int i = 0; i < currentSpellBindingDictionary.Count; i++) {
			if (currentSpellBindingDictionary [i] != null) {
				Debug.Log ("PlayerSpellComponent: OnEquipStaff " + puzzleGameData.puzzleData.puzzleName + " CurrentSpellBindingDictionary " + i + " not null. setting up spell UI.");
				currentSpellBindingDictionary [i].spellUI = playerObject.playerUI.spellUIs [i];
				playerObject.playerUI.spellUIs [i].SetSpellUIToSpell (currentSpellBindingDictionary [i].spellData);
			} else {
				playerObject.playerUI.spellUIs [i].ClearSpellBinding ();
			}
		}

	}

	public override void OnDropStaff (PuzzleKey region, PuzzleGameData puzzleGameData) {

		if (puzzleGameData.spellBindingDictionary == currentSpellBindingDictionary) {
			currentSpellBindingDictionary = null;
		}

		for (int i = 0; i < playerObject.playerUI.spellUIs.Length; i++) {
			playerObject.playerUI.spellUIs [i].ClearSpellBinding ();
		}

		playerObject.playerUI.OnDropStaff (region);
	}
	public override void OnBindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGemGameData, PuzzleBindType bindType) {
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
				spellGemGameData.spellCast.spellUI.SetSpellUIToSpell (spellGemGameData.spellData);
			}
		}
		spellGemGameData.spellCast.ConfigureSpellToPlayer (playerObject);
		spellGemGameData.spellCast.tag = puzzleGameData.puzzleEntity.tag;
	}

	public override void OnUnbindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellSaveData, PuzzleUnbindType unbindType) {
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
		if (currentSpellBindingDictionary == null || currentSpellBindingDictionary [spellIndex] == null)
			return;
		else {
			Spell spell = currentSpellBindingDictionary [spellIndex];
			spell.SpellButtonDown ();
		}
	}
	public void OnSpellButton (int spellIndex) {
		//channel spell
		if (currentSpellBindingDictionary == null || currentSpellBindingDictionary [spellIndex] == null)
		return;

		Spell spell = currentSpellBindingDictionary [spellIndex];
		spell.SpellButtonHold ();
	}
	public void OnSpellButtonUp (int spellIndex) {
		if (currentSpellBindingDictionary == null || currentSpellBindingDictionary [spellIndex] == null)
			return;

		Spell spell = currentSpellBindingDictionary [spellIndex];
		spell.SpellButtonUp ();

	}
}