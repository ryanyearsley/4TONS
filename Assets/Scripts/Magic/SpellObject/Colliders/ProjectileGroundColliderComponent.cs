using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGroundColliderComponent : MonoBehaviour {
	private ProjectileObject projectileObject;
	private void Awake () {
		projectileObject = GetComponentInParent<ProjectileObject> ();
	}
	protected virtual void OnTriggerEnter2D (Collider2D other) {
		Debug.Log ("projectile hit environment: " + other.gameObject.name);
		projectileObject.OnWallHit ();
	}
}