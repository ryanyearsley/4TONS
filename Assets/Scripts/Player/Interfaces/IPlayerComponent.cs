using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerComponent {
	void ReusePlayerComponent (Player player);
	void OnChangePlayerState (PlayerState playerState);
	void OnDash (DashInfo dashInfo);

	//void OnCastSpell (Spell spell);
	//void OnChannelSpell (Spell spell);
	//void OnEndSpell (Spell spell);
	void OnBindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGemGameData,PuzzleBindType bindType);
	void OnUnbindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGemGameData, PuzzleUnbindType unbindType);
}
