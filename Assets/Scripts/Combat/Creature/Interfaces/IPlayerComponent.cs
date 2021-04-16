using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerComponent {
	void ReusePlayerComponent (Player player);
	void OnChangePlayerState (PlayerState playerState);
	void OnDash (DashInfo dashInfo);

	void OnBindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGemGameData);
	void OnUnbindSpellGem (PuzzleGameData puzzleGameData, SpellGemGameData spellGemGameData);
}
