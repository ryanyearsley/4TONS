using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExamplePlayerComponent : PlayerComponent
{

	public override void OnDash (DashInfo dashInfo) {

	}
	public override void OnPickUpStaff (PuzzleKey region, PuzzleGameData puzzleGameData) {

	}
	public override void OnDropStaff (PuzzleKey region, PuzzleGameData puzzleGameData) {

	}
	public override void OnEquipStaff (PuzzleKey region, PuzzleGameData puzzleGameData, StaffEquipType equipType) {

	}

	public override void OnPickUpSpellGem (SpellGemGameData spellGemGameData) {

	}
	public override void OnDropSpellGem (SpellGemGameData spellGemGameData) {

	}
	public override void OnRotateSpellGem (SpellGemGameData spellGemGameData, int rotateIndex) {

	}

	public override void OnBindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGameData, PuzzleBindType bindType) {

	}

	public override void OnUnbindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGameData, PuzzleUnbindType unbindType) {

	}

}
