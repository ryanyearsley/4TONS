using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierObject : SpellObject {

	[SerializeField]
	private float maxValue = 150;
	private float currentValue;

	public override void ReuseSpellObject (VitalsEntity vitalsEntity) {
		base.ReuseSpellObject (vitalsEntity);
		currentValue = maxValue;
	}
	public virtual void DamageBarrier (float damage) {
			float realValue = currentValue - damage;
			Debug.Log ("barrier receiving " + damage + " damage. current HP: " + currentValue + ", projected value: " + realValue);
			currentValue = Mathf.Clamp (realValue, 0, maxValue);
			if (currentValue < 0) {
				Destroy ();
			}
	}


}
