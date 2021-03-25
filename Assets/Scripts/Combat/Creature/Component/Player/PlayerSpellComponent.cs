using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpellComponent : PlayerComponent {

	[SerializeField]
	private GameObject spellBindUIPrefab;

	private ResourceComponent manaController;
	private SpellBindUI spellBindUI;

	[SerializeField]
	private SpellBindingDictionary spellBindings = new SpellBindingDictionary();
	private int spellSlots = 4;

	public override void SetUpComponent (GameObject rootObject) {
		base.SetUpComponent (rootObject);
		manaController = rootObject.GetComponentInChildren<ResourceComponent> ();
		GameObject spellBindUIGo = Instantiate (spellBindUIPrefab);
		spellBindUI = spellBindUIGo.GetComponent<SpellBindUI> ();
		spellBindUIGo.transform.parent = rootObject.transform.root;
		spellBindUI.InitializeSpellBindUI ();
		for (int i = 0; i < spellSlots; i++) {
			spellBindings.Add (i, null);
		}
	}
	public override void ReusePlayerComponent (Player player) {
		base.ReusePlayerComponent (player);
		ClearSpellBindings ();
	}

	public override void OnSpawn (Vector3 spawnPosition) {
		base.OnSpawn (spawnPosition);
		playerObject.SetCanAttack (true);
	}

	public override void OnChangePlayerState (PlayerState playerState) {
		switch (playerState) {
			case (PlayerState.DEAD):
			case (PlayerState.COMBAT): {
					spellBindUI.gameObject.SetActive (false);
					break;
				}
			case (PlayerState.PUZZLE_BROWSING): {
					spellBindUI.gameObject.SetActive (true);
					break;
				}
			case (PlayerState.PUZZLE_MOVING_SPELLGEM): {
					spellBindUI.gameObject.SetActive (true);
					break;
				}
		}
	}

	public override void OnBindSpellGem (PuzzleGroupingDetails details, SpellSaveData spellSaveData) {
		Debug.Log (" PlayerSpell On BindSpellGem");
		if (spellSaveData.spellCast == null){
			GameObject spellCastGO = Instantiate(spellSaveData.spellData.castObject);
			spellSaveData.spellCast = spellCastGO.GetComponent<Spell>();
		}
		if (details.puzzleType == PuzzleGroupingType.EQUIPPED_STAFF)
			CalculateSpellBind (details, spellSaveData);
		spellSaveData.spellCast.transform.parent = details.castParentTransform;
		spellSaveData.spellCast.tag = this.tag;
		spellSaveData.spellCast.transform.localPosition = Vector3.zero;
	}

	public override void OnUnbindSpellGem (PuzzleGroupingDetails details, SpellSaveData spellSaveData) {
		if (details.puzzleType == PuzzleGroupingType.EQUIPPED_STAFF) {
			spellSaveData.spellCast.spellUI = null;
			playerObject.OnUpdateSpellBinding (spellSaveData.spellIndex, null);
		}
		spellSaveData.spellCast.transform.parent = PoolManager.instance.transform;
		spellSaveData.spellCast.tag = "Untagged";

		spellSaveData.spellCast.transform.localPosition = Vector3.zero;
	}
	private void CalculateSpellBind (PuzzleGroupingDetails details, SpellSaveData spellSaveData) {
		if (details.puzzleType != PuzzleGroupingType.EQUIPPED_STAFF) {
			Debug.Log ("No need to bind. Not the equipped staff.");
		} else if (!spellBindings.ContainsKey (spellSaveData.spellIndex)) {
			Debug.Log ("Invalid binding: Binding index not in binding dictionary.");
		} else if (spellBindings[spellSaveData.spellIndex] == spellSaveData.spellCast) {
			Debug.Log ("Spell already bound to that index.");
		} else if (spellBindings[spellSaveData.spellIndex] != null) {
			Debug.Log ("Invalid binding: Key already bound. Auto-assigning valid binding index.");
			int freeSpellIndex = AutoAssignBinding ();
			if (freeSpellIndex <= 3) {
				spellSaveData.spellIndex = freeSpellIndex;
				playerObject.OnUpdateSpellBinding (spellSaveData.spellIndex, spellSaveData.spellCast);
			}
		} else {
			Debug.Log ("Binding available! Binding key.");
			playerObject.OnUpdateSpellBinding (spellSaveData.spellIndex, spellSaveData.spellCast);
		}
	}
	public override void OnUpdateSpellBindingEvent (int spellIndex, Spell spell) {
		spellBindings [spellIndex] = spell;
		spellBindUI.UpdateBindingUI (spellIndex, spell);
		if (spell == null) {
			UIManager.Instance.playerUIs [playerObject.player.playerIndex].spellUIs [spellIndex].UnbindSpellUI ();
		}
		if (spellBindings [spellIndex] != null) {
			spellBindings [spellIndex].spellUI = UIManager.Instance.playerUIs [playerObject.player.playerIndex].spellUIs [spellIndex];
			spellBindings [spellIndex].spellUI.InitializeSpellUI (spell.spellData);
		}
	}

	private int AutoAssignBinding() {

		for (int i = 0; i < spellBindings.Count; i++) {
			if (spellBindings [i] == null) {
				return i;
			}
		}
		return 10;
	}

	//Used on reuse of a player object, as well as when switching weapons.
	private void ClearSpellBindings () {
		for (int i = 0; i < spellSlots; i++) {
			spellBindings [i] = null;
		}
	}

	public void OnSpellButtonDown (int spellIndex) {
		if (!playerObject.canAttack || spellBindings[spellIndex] == null)
			return;
		Spell spell = spellBindings[spellIndex];
		if (!spell.onCooldown &&
			manaController.SubtractResourceCost (spell.spellData.manaCost) &&
			spell.isCastEligible()) {
			spell.CastSpell ();
			playerObject.OnAttack (new AttackInfo(spell.spellData.castTime, spell.spellData.castSpeedReduction));
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