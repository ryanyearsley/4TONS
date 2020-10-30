using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellController : MonoBehaviour {

	[SerializeField]
	private StaffData staffData;

	private PlayerStateController stateController;
	private ManaController manaController;
	private bool canCast;

	[SerializeField]
	private Spell[] spells;

	//SPELL GEM AND PUZZLE VARIABLES
	private PuzzleUI puzzleUI;

	//active list of spell gems within pick-up distance.
	private List<SpellGemPickup> interactableSpellGemPickups = new List<SpellGemPickup>();

	private void Awake () {
		manaController = GetComponentInChildren<ManaController> ();
		GameObject spellGemCanvasGo = Instantiate (ConstantsManager.instance.inventoryPuzzleUIPrefab);
		spellGemCanvasGo.transform.parent = this.transform;
		spellGemCanvasGo.transform.localPosition = Vector3.up * 2.5f;
		puzzleUI = spellGemCanvasGo.GetComponent<PuzzleUI> ();
		spellGemCanvasGo.SetActive (false);
	}

	public void OnSpellButtonDown (int spellIndex) {
		if (!canCast || spells.Length - 1 < spellIndex)
			return;

		Debug.Log ("Spell down: " + spellIndex);
		PlayerPositions positions = stateController.playerPositions;
		Spell spell = spells[spellIndex];
		if (!spell.onCooldown &&
			manaController.SubtractManaCost (spell.manaCost)) {
			Debug.Log ("SpellController casting spell");
			spell.CastSpell ();
			stateController.AddDebuff (new DebuffInfo (0.5f, 0.5f, false));
		}
	}

	public void OnSpellButton (int spellIndex) {
		//channel spell
		if (!canCast)
			return;

		PlayerPositions positions = stateController.playerPositions;
	}

	public void OnSpellButtonUp (int spellIndex) {
		if (!canCast)
			return;

		PlayerPositions positions = stateController.playerPositions;
	}

	public void SetCanCast (bool canCast) {
		this.canCast = canCast;
	}

	private void OnEnable () {
		stateController = GetComponent<PlayerStateController> ();
		stateController.OnSetCanCastEvent += SetCanCast;
	}

	private void OnDisable () {
		stateController.OnSetCanCastEvent -= SetCanCast;
	}

	public void AddSpellGemToInteractable (SpellGemPickup spellGemPickup) {
		interactableSpellGemPickups.Add (spellGemPickup);

	}
	public void RemoveSpellGemFromInteractable (SpellGemPickup spellGemPickup) {
		interactableSpellGemPickups.Remove (spellGemPickup);
	}
	public void AttemptSpellPickup() {
		if (interactableSpellGemPickups.Count == 1) {

		}
		else if (interactableSpellGemPickups.Count > 1) {
			CalculateClosestSpellGem ();
		}
	}
	private SpellGemPickup CalculateClosestSpellGem () {
		SpellGemPickup closestSpellGemPickup = null;
		float closestDistance = 5;
		foreach (SpellGemPickup spellGemPickup in interactableSpellGemPickups) {
			float distance = Vector2.Distance(spellGemPickup.transform.position, transform.position);
			if (closestSpellGemPickup == null || distance < closestDistance) {
				closestSpellGemPickup = spellGemPickup;
				closestDistance = distance;
			}
		}
		return closestSpellGemPickup;
	}
}