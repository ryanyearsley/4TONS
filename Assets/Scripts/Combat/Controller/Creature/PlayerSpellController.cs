using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpellController : MonoBehaviour {


	private PlayerStateController playerStateController;
	private ManaController manaController;
	private bool canCast;

	[SerializeField]
	private SpellBindingDictionary spellBindings = new SpellBindingDictionary();
	private int spellSlots = 4;

	private void Awake () {
		manaController = GetComponentInChildren<ManaController> ();
		for (int i = 0; i < spellSlots; i++) {
			spellBindings.Add (i, null);
		}
	}

	public void OnBindSpellGem (PuzzleGroupingDetails details, SpellSaveData spellSaveData) {
		if (spellSaveData.spellCast == null){
			GameObject spellCastGO = Instantiate(spellSaveData.spellData.castObject);
			spellSaveData.spellCast = spellCastGO.GetComponent<Spell>();

		}
		if (details.puzzleType == PuzzleGroupingType.EQUIPPED_STAFF)
			CalculateSpellBind (details, spellSaveData);
		spellSaveData.spellCast.transform.parent = details.castParentTransform;
		spellSaveData.spellCast.transform.localPosition = Vector3.zero;
	}

	public void OnUnbindSpellGem (PuzzleGroupingDetails details, SpellSaveData spellSaveData) {
		if (details.puzzleType == PuzzleGroupingType.EQUIPPED_STAFF) {
			playerStateController.OnUpdateSpellBinding (spellSaveData.spellIndex, null);
		}
		spellSaveData.spellCast.transform.parent = PoolManager.instance.transform;
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
				playerStateController.OnUpdateSpellBinding (spellSaveData.spellIndex, spellSaveData.spellCast);
			}
		} else {
			Debug.Log ("Binding available! Binding key.");
			playerStateController.OnUpdateSpellBinding (spellSaveData.spellIndex, spellSaveData.spellCast);
		}
	}
	private void OnUpdateSpellBinding (int spellIndex, Spell spell) {
			spellBindings [spellIndex] = spell;
	}

	private int AutoAssignBinding() {

		for (int i = 0; i < spellBindings.Count; i++) {
			if (spellBindings [i] == null) {
				return i;
			}
		}
		return 10;
	}

	public void OnSpellButtonDown (int spellIndex) {
		if (!canCast || spellBindings[spellIndex] == null)
			return;

		Debug.Log ("Spell down: " + spellIndex);
		CreaturePositions positions = playerStateController.creaturePositions;
		Spell spell = spellBindings[spellIndex];
		if (!spell.onCooldown &&
			manaController.SubtractManaCost (spell.spellData.manaCost)) {
			Debug.Log ("SpellController casting spell");
			spell.CastSpell ();
			playerStateController.OnCastSpell (spell.spellData);
			playerStateController.AddDebuff (new DebuffInfo (0.5f, 0.5f, false));
		}
	}

	public void OnSpellButton (int spellIndex) {
		//channel spell
		if (!canCast)
			return;

		CreaturePositions positions = playerStateController.creaturePositions;
	}

	public void OnSpellButtonUp (int spellIndex) {
		if (!canCast)
			return;

		CreaturePositions positions = playerStateController.creaturePositions;
	}

	public void SetCanCast (bool canCast) {
		this.canCast = canCast;
	}

	private void OnEnable () {
		playerStateController = GetComponentInParent<PlayerStateController> ();
		playerStateController.OnSetCanAttackEvent += SetCanCast;
		playerStateController.OnBindSpellGemEvent += OnBindSpellGem;
		playerStateController.OnUnbindSpellGemEvent += OnUnbindSpellGem;
		playerStateController.OnUpdateSpellBindingEvent += OnUpdateSpellBinding;
	}

	private void OnDisable () {
		playerStateController.OnSetCanAttackEvent -= SetCanCast;
		playerStateController.OnBindSpellGemEvent -= OnBindSpellGem;
		playerStateController.OnUnbindSpellGemEvent -= OnUnbindSpellGem;
		playerStateController.OnUpdateSpellBindingEvent -= OnUpdateSpellBinding;
	}
}