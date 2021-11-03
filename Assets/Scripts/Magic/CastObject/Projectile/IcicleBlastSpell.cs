using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcicleBlastSpell : Spell {
	[SerializeField]
	private int icicleCount = 3;


	public override void CastSpell () {
		base.CastSpell ();
		for (int i = 0; i < icicleCount; i++) {
			PoolManager.instance.ReuseSpellObject (spellData.spellObject, spellCastTransform.position, spellCastTransform.rotation, playerObject.vitalsEntity);
		}
	}
}
