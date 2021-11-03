using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NapalmDeathProjectile : ProjectileObject {
	[SerializeField]
	private GameObject napalmDebrisObject;
	[SerializeField]
	private int napalmDebrisCount;
	[SerializeField]
	private float explosionRadius;
	public override void SetupObject () {
		base.SetupObject ();
		PoolManager.instance.CreateSpellObjectPool (napalmDebrisObject, napalmDebrisCount * 3);
	}
/*
	public override void TerminateObjectFunctions () {
		Vector3 pointOfContact = trans.position;

		if (napalmDebrisObject != null) {

			for (int i = 0; i < napalmDebrisCount; i++) {
				Vector3 napalmPosition = pointOfContact + new Vector3 (Random.Range (-explosionRadius, explosionRadius),
				Random.Range (-explosionRadius/2, explosionRadius/2), 0);
				
				PoolManager.instance.ReuseSpellObject (napalmDebrisObject, napalmPosition, Quaternion.identity, casterVitalsEntity);
			}
		}
		LevelManager.instance.DestroyEnvironment (pointOfContact, Mathf.RoundToInt(explosionRadius));
		
		Destroy ();
	}*/
}
