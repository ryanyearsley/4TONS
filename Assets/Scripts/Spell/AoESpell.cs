using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoESpell : Spell {

	public LayerMask aoeMask;
	public override bool isCastEligible () {
		if (onCooldown)
			return false;
		else if (Physics2D.OverlapCircle (spellCastTransform.position, 0.1f, aoeMask)) {
			Debug.Log ("invalid cast position. Cannot cast AoE. ");
			return false;
		}
		Debug.Log ("AoE can cast - raycast didn't hit object on layer mask");
		return true;
	}
	public override void CastSpell () {
			base.CastSpell ();
			PoolManager.instance.ReuseSpellObject (spellData.spellObject, 
				new Vector3 (spellCastTransform.position.x, spellCastTransform.position.y, 0f), 
				Quaternion.identity, stateController.vitalsEntity);
	}


	public void OnDrawGizmos () {
		Debug.DrawRay (spellCastTransform.position, spellCastTransform.TransformDirection (Vector3.forward * 20), Color.yellow);

	}
}
