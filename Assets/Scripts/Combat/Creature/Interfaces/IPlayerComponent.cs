using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerComponent {
	void ReusePlayerComponent (Player player);
	void OnChangePlayerState (PlayerState playerState);
	void OnDash (DashInfo dashInfo);
	void OnBindSpellGem (PuzzleGroupingDetails details, SpellSaveData spellSaveData);
	void OnUnbindSpellGem (PuzzleGroupingDetails details, SpellSaveData spellSaveData);
	void OnUpdateSpellBindingEvent (int spellBindIndex, Spell spell);
}
