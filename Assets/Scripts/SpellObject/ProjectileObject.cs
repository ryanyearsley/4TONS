using System.Linq;
using UnityEngine;

public class ProjectileObject : SpellObject {
	[SerializeField]
	private float moveSpeed;

	[SerializeField]
	private Collider2D groundColl;

	[SerializeField]
	private ContactFilter2D contactFilter = new ContactFilter2D();

	[SerializeField]
	private GameObject debrisObject;
	private Collider2D[] overlappingColliders = new Collider2D[16];
	void Awake () {
		if (debrisObject != null)
			PoolManager.instance.CreateObjectPool (debrisObject, 3);
		trans = transform;
	}

	public virtual void FixedUpdate () {

		if (isAlive) {
			transform.Translate (Vector2.right * moveSpeed * Time.fixedDeltaTime);

			Vector3 projectedEnvironmentColliderPosition = new Vector3 (trans.position.x, trans.position.y - 0.18f, 0);
			groundColl.transform.position = projectedEnvironmentColliderPosition;
			if (Physics2D.OverlapCollider (groundColl, contactFilter, overlappingColliders) > 0) {
				if (debrisObject != null) {
					Debug.Log ("projectile ground overlap collider reusing debris object");
					PoolManager.instance.ReuseObject (debrisObject, groundColl.transform.position, Quaternion.identity);
				}
				Destroy ();
			}
		}

	}
	private void OnTriggerEnter2D (Collider2D other) {
		int id = other.transform.parent.GetInstanceID();
		Debug.Log (this.tag + " tagged projectile collided other object. name: " + other.gameObject.name);
		if (isAlive) {
			if (other.tag != this.tag) {
				VitalsEntity hitTargetEntity = VitalsManager.Instance.GetVitalsEntitybyID(id);
				if (hitTargetEntity != null && hitTargetEntity != casterVitalsEntity) {

					Debug.Log ("projectile hit valid target. ID:" + id);
					if (hitTargetEntity.health != null)
						hitTargetEntity.health.ApplyDamage (spellObjectData.damage);

					if (hitTargetEntity.resource != null)
						hitTargetEntity.resource.ApplyResourceDamage (spellObjectData.manaDamage);

					if (casterVitalsEntity.health != null)
						casterVitalsEntity.health.Heal (spellObjectData.casterHealthHealAmount);

					if (casterVitalsEntity.resource != null)
						casterVitalsEntity.resource.RegenerateMana (spellObjectData.casterManaRegenAmount);

				}
				if (debrisObject != null) {
					Debug.Log ("projectile reusing debris object");
					PoolManager.instance.ReuseObject (debrisObject, groundColl.transform.position, Quaternion.identity);
				}
				Destroy ();
			}
		}

	}
}
