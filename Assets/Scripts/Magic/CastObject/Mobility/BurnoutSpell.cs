using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnoutSpell : Spell {

	public float duration;
	public int flameEmitCount;

	public override void CastSpell () {
		base.CastSpell ();
		StartCoroutine (BurnoutRoutine ());
	}

	public IEnumerator BurnoutRoutine() {
		int flameCount = 0;
		float flameEmitInterval = duration/flameEmitCount;
		while (flameCount < flameEmitCount) {
			float playerDirectionAngle = Mathf.Atan2 (playerObject.velocity.y, playerObject.velocity.x) * Mathf.Rad2Deg;
			PoolManager.instance.ReuseSpellObject (spellData.spellObject, spellCastTransform.position, Quaternion.AngleAxis (playerDirectionAngle + 180, Vector3.forward), playerObject.vitalsEntity);
			yield return new WaitForSeconds (flameEmitInterval);
			flameCount++;
		}
	}
}
