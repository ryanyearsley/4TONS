using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitObjectSpellEffect : SpellEffect
{

	[SerializeField]
	public float emitInterval = 0.2f;
	private float emitTimer;
	[SerializeField]
	public Transform emitPointTransform;
	[SerializeField]
	public GameObject emitObjectPrefab;
	[SerializeField]
	public int emitObjectPoolSize = 5;
	public override void SetUpSpellEffect () {
		PoolManager.instance.CreateSpellObjectPool (emitObjectPrefab, emitObjectPoolSize);

	}
	public override void UpdateSpellEffect (VitalsEntity caster) {
		base.UpdateSpellEffect (caster);
		emitTimer += Time.deltaTime;
		if (emitTimer > emitInterval) {
			emitTimer = 0; 
			PoolManager.instance.ReuseSpellObject (emitObjectPrefab, emitPointTransform.position, emitPointTransform.rotation, caster);
		}

	}
}
