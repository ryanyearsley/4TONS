using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiShotSpell : ProjectileSpell
{

	[SerializeField]
	private float spread;
	[SerializeField]
	private int shotCount;
	[SerializeField]
	private float shotInterval;

	public override void CastSpell () {
		playerObject.OnCastSpell (this, SpellCastType.CAST);
		cdTimer = spellData.coolDown;
		spellUI.GreyOutSpellUI ();
		onCooldown = true;
		StartCoroutine (MultiShotRoutine ());
    }

	public override void CancelCast () {
		base.CancelCast ();
		StopCoroutine (MultiShotRoutine());
	}

	public IEnumerator MultiShotRoutine () {
		for (int i = 0; i < shotCount; i++) {
			float randomRotation = Random.Range(-spread, spread);
			Quaternion shotRotation = Quaternion.Euler(0, 0, spellCastTransform.rotation.eulerAngles.z + randomRotation);
			PoolManager.instance.ReuseSpellObject (spellData.spellObject, spellCastTransform.position, shotRotation, playerObject.vitalsEntity);
			yield return new WaitForSeconds (shotInterval);
		}
	}
}
