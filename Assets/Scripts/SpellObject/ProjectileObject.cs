using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileObject : SpellObject {
	[SerializeField]
	private float moveSpeed;

	[SerializeField]
	private float hitboxRadius = 0.15f;

	private Transform trans;
	private Animator animator;

	[SerializeField]
	private LayerMask environmentMask;
	[SerializeField]
	private GameObject debrisObject;


	void Awake () {
		PoolManager.instance.CreateObjectPool (debrisObject, 3);
		trans = transform;
		animator = GetComponentInChildren<Animator> ();
	}
	public virtual void FixedUpdate () {

		transform.Translate (Vector2.right * moveSpeed * Time.fixedDeltaTime);

		Vector3 projectedEnvironmentColliderPosition = new Vector3 (trans.position.x, trans.position.y - 0.25f, 0);
		if (Physics2D.OverlapCircle (projectedEnvironmentColliderPosition, hitboxRadius, environmentMask)) {
			PoolManager.instance.ReuseObject (debrisObject, this.transform.position, Quaternion.identity);
			Destroy ();
		}

	}
	private void OnTriggerEnter2D (Collider2D other) {
		int id = other.transform.parent.GetInstanceID();
		Debug.Log ("projectile collided with " + id + ", object name: " + other.gameObject.name);
		if (other.tag != this.tag) {
			VitalsEntity hitTargetEntity = VitalsManager.Instance.GetVitalsEntitybyID(id);
			if (hitTargetEntity != null) {

				Debug.Log ("projectile hit valid target. ID:" + id);
				if (hitTargetEntity.health != null )
					hitTargetEntity.health.ApplyDamage (spellObjectData.damage);

				if (hitTargetEntity.resource != null)
					hitTargetEntity.resource.ApplyResourceDamage (spellObjectData.manaDamage);

				if (casterVitalsEntity.health != null)
					casterVitalsEntity.health.Heal (spellObjectData.casterHealthHealAmount);

				if (casterVitalsEntity.resource != null)
					casterVitalsEntity.resource.RegenerateMana (spellObjectData.casterManaRegenAmount);

			}

			PoolManager.instance.ReuseObject (debrisObject, this.transform.position, Quaternion.identity);
			Destroy ();
		}

	}
}
