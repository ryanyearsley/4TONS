using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCastUtility {

	public static void LoadSpellCastEntity (PuzzleGameData puzzleGameData, SpellGemGameData spellGemGameData) {
		Debug.Log ("PlayerSpell On BindSpellGem");
		if (spellGemGameData.spellCast == null) {
			spellGemGameData.spellCast = GameObject.Instantiate (spellGemGameData.spellData.castObject).GetComponent<Spell> ();
			spellGemGameData.spellCast.transform.parent = SpellCastObjectManager.instance.transform;
			spellGemGameData.spellCast.transform.localPosition = Vector3.zero;
		}
		if (puzzleGameData.puzzleData.puzzleType != PuzzleType.INVENTORY) {
			puzzleGameData.spellBindingDictionary [spellGemGameData.spellBindIndex] = spellGemGameData.spellCast;
		}
	}


}
