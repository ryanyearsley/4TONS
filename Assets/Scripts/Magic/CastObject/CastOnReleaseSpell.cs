using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This spell type casts a projection of the spell to target area when button is held, 
//and casts the spell on release.
public class CastOnReleaseSpell : Spell {

	[SerializeField]
	private float maxRange = 8;
	[SerializeField]
	private Transform previewObjectTransform;
	protected SpriteRenderer previewObjectSprite;



	public LayerMask aoeMask;


	public override void SpellButtonDown () {
		if (playerObject.currentPlayerState == PlayerState.COMBAT) {

			if (!playerObject.usingMouseControls && playerObject.smartCursor)
				playerObject.SetAimingMode (AimingMode.CURSOR);

			previewObjectTransform.gameObject.SetActive (true);
			previewObjectTransform.position = spellCastTransform.position;
		}
	}
	public override void SpellButtonHold () {


		if (playerObject.currentPlayerState == PlayerState.COMBAT) {

			previewObjectTransform.position = spellCastTransform.position;

			if (isCastEligible ()) {
				previewObjectSprite.color = ConstantsManager.instance.validProjectedAoEColor;
			} else {
				previewObjectSprite.color = ConstantsManager.instance.invalidProjectedAoEColor;
			}
		}
	}

	public override void SpellButtonUp () {
		if (isCastEligible ()) {
			CastSpell ();
		}

		if (!playerObject.usingMouseControls && playerObject.smartCursor)
			playerObject.SetAimingMode (AimingMode.RADIAL);


		previewObjectTransform.localPosition = Vector3.zero;
		previewObjectTransform.gameObject.SetActive (false);

	}
	public override void SetUpSpell () {
		base.SetUpSpell ();
		previewObjectTransform.localPosition = Vector2.zero;
		previewObjectSprite = previewObjectTransform.GetComponentInChildren<SpriteRenderer> ();
		previewObjectTransform.gameObject.SetActive (false);
	}

	public override bool isCastEligible () {
		if (playerObject.currentPlayerState != PlayerState.COMBAT
			|| onCooldown
			|| !playerObject.canAttack
			|| !playerObject.vitalsEntity.resource.HasEnoughMana (spellData.manaCost)
			|| maxRange < playerObject.DistanceToTarget ())
			return false;
		else if (Physics2D.OverlapCircle (spellCastTransform.position, 0.1f, aoeMask)) {
			Debug.Log ("CastOnReleaseSpell: invalid cast position. Cannot cast AoE. ");
			return false;
		}
		Debug.Log ("CastOnReleaseSpell: AoE can cast - raycast didn't hit object on layer mask");
		return true;
	}
	public override void CastSpell () {
		base.CastSpell ();
		Debug.Log ("CastOnReleaseSpell: CastSpell()");
		if (spellData.spellObject != null) {
			PoolManager.instance.ReuseSpellObject (spellData.spellObject,
				new Vector3 (spellCastTransform.position.x, spellCastTransform.position.y, 0f),
				Quaternion.identity, playerObject.vitalsEntity);
		}
	}
}
