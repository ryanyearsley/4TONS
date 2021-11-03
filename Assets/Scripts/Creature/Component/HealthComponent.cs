
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : VitalsComponent {

	public bool isPlayer = false;
	public GameObject bloodPoolPrefab;
	public override void SetUpComponent (GameObject rootObject) {
		Debug.Log ("Health component setup");
		base.SetUpComponent (rootObject);
		maxValue = creatureObject.creatureData.maxHealth;
		PoolManager.instance.CreateObjectPool (bloodPoolPrefab, 1);
	}
	public override void OnDeath () {
		PoolManager.instance.ReuseObject (bloodPoolPrefab, transform.position, Quaternion.identity);
		base.OnDeath ();
	}

	public void ApplyDamage (float damage) {
		if (!creatureObject.isDead) {
			float realValue = currentValue - damage;
			Debug.Log ("creature receiving " + damage + " damage. current HP: " + currentValue + ", projected value: " + realValue);
			currentValue = Mathf.Clamp (realValue, 0, maxValue);
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
