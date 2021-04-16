
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : VitalsComponent {

	public bool isPlayer = false;
	public GameObject bloodPoolPrefab;
	public override void SetUpComponent (GameObject rootObject) {
		Debug.Log ("Health component setup");
		base.SetUpComponent (rootObject);
		maxValue = creatureObject.creatureData.health;
		PoolManager.instance.CreateObjectPool (bloodPoolPrefab, 2);
	}

	public override void OnDeath () {
		PoolManager.instance.ReuseObject (bloodPoolPrefab, transform.position, Quaternion.identity);
		base.OnDeath ();
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
