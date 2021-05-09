using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoESpell : Spell {

	[SerializeField]
	private Transform previewObjectTransform;
	private SpriteRenderer previewObjectSprite;



	public LayerMask aoeMask;


	public override void SpellButtonDown () {
		if (!playerObject.usingMouseControls && playerObject.smartCursor)
			playerObject.SetAimingMode (AimingMode.CURSOR);

		if (isCastEligible()) {
			previewObjectTransform.gameObject.SetActive (true);
			previewObjectTransform.position = spellCastTransform.position;
		}
	}
	public override void SpellButtonHold () {
		previewObjectTransform.position = spellCastTransform.position;

		if (isCastEligible ()) {
			previewObjectSprite.color = ConstantsManager.instance.validProjectedAoEColor;
		} else {
			previewObjectSprite.color = ConstantsManager.instance.invalidProjectedAoEColor;
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
		previewObjectTransform.localPosition = Vector2.zero;
		previewObjectSprite = previewObjectTransform.GetComponentInChildren<SpriteRenderer> ();
		previewObjectTransform.gameObject.SetActive (false);
	}

	public override bool isCastEligible () {
		if (onCooldown || !playerObject.canAttack || !playerObject.vitalsEntity.resource.HasEnoughMana (spellData.manaCost))
			return false;
		else if (Physics2D.OverlapCircle (spellCastTransform.position, 0.1f, aoeMask)) {
			Debug.Log ("invalid cast position. Cannot cast AoE. ");
			return false;
		}
		Debug.Log ("AoE can cast - raycast didn't hit object on layer mask");
		return true;
	}
	public override void ChannelSpell () {
		base.ChannelSpell ();
	}
	public override void CastSpell () {
		base.CastSpell ();
		PoolManager.instance.ReuseSpellObject (spellData.spellObject,
			new Vector3 (spellCastTransform.position.x, spellCastTransform.position.y, 0f),
			Quaternion.identity, playerObject.vitalsEntity);
	}


	public void OnDrawGizmosSelected () {
		Debug.DrawRay (spellCastTransform.position, spellCastTransform.TransformDirection (Vector3.forward * 20), Color.yellow);

	}
}
