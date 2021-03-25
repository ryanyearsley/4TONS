
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : VitalsComponent {

	public override void SetUpComponent (GameObject rootObject) {
		Debug.Log ("Health component setup");
		base.SetUpComponent (rootObject);
		maxValue = creatureObject.creatureData.health;
	}

	public float GetHealth () {
		return currentValue;
	}

	public void ApplyDamage (float damage) {
		if (!creatureObject.isDead) {
			currentValue = Mathf.Clamp (currentValue -= damage, 0, maxValue);
			UpdateVitalsBar ();
			if (currentValue > 0) {
				creatureObject.OnHit (new HitInfo (damage, Vector2.zero, currentValue));
			} else {
				creatureObject.OnDeath ();
			}
		}
	}
	public void Heal (float healAmount) {
		if (!creatureObject.isDead) {
			currentValue = Mathf.Clamp (currentValue += healAmount, 0, maxValue);
			UpdateVitalsBar ();
		}
	}
}
