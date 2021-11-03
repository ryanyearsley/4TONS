using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticProjectileSpell : ProjectileSpell {
	public override void SpellButtonHold () {
		if (isCastEligible ()) {
			CastSpell ();
		}
	}
	public override void CastSpell () {
		playerObject.OnCastSpell (this, SpellCastType.CAST);
		cdTimer = spellData.coolDown;
		spellUI.GreyOutSpellUI ();
		onCooldown = true; 
		PoolManager.instance.ReuseSpellObject (spellData.spellObject, spellCastTransform.position, spellCastTransform.rotation, playerObject.vitalsEntity);
	}
}
