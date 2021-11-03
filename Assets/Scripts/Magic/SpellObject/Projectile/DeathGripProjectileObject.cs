using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathGripProjectileObject : ProjectileObject {

	private SpeedAlteringEffect currentPullSpeedEffect;

	[SerializeField]
	private Transform chainPointTransform;
	[SerializeField]
	private LineRenderer lineRenderer;

	private bool isPulling = false;
	private Transform anchorTransform;
	private Transform cursorTransform;
	public override void ReuseSpellObject (VitalsEntity casterVitals) {
		base.ReuseSpellObject (casterVitals);
		anchorTransform = casterVitals.creatureObject.creaturePositions.staffAimTransform;
		cursorTransform = casterVitals.creatureObject.creaturePositions.targetTransform;

		lineRenderer.SetPosition (0, anchorTransform.position);
		lineRenderer.SetPosition (1, chainPointTransform.position);
		lineRenderer.enabled = true;
		isPulling = false;
		
	}
	public override void FixedUpdate () {
		if (isAlive) {
			lineRenderer.SetPosition (0, anchorTransform.position);
			lineRenderer.SetPosition (1, chainPointTransform.position);
			if (!isPulling) {
				transform.Translate (Vector2.right * moveSpeed * Time.fixedDeltaTime);

			} 
			else {
				Vector2 handDirection = ((Vector2)anchorTransform.position - (Vector2)chainPointTransform.position).normalized;
				float angle = Mathf.Atan2 (handDirection.y, handDirection.x) * Mathf.Rad2Deg;
				trans.rotation = Quaternion.AngleAxis (angle + 180, Vector3.forward);
				currentPullSpeedEffect.velocityOverride = transform.right * -moveSpeed;
				transform.Translate (Vector2.left * moveSpeed * Time.fixedDeltaTime);
			}
		}
	}

public override void OnWallHit () {
	Destroy ();
}
public override void OnEnemyHit (VitalsEntity enemyVitals) {
	if (!isPulling) {
		ApplyPullEffect (enemyVitals);
		isPulling = true;
		float hitTime = lifeTimer;
		lifeTimer = lifeTime - hitTime;
	}
}

public void ApplyPullEffect (VitalsEntity vitals) {
	if (vitals.movement != null) {
		SpeedAlteringEffect speedAlteringEffect = new SpeedAlteringEffect (1, lifeTimer, false);
		speedAlteringEffect.isVelocityOverride = true;
		Vector2 oppositeDirection = transform.right * -moveSpeed * 2;
		Debug.Log ("PullEnemyToCaster: OppositeDirection: " + oppositeDirection);
		speedAlteringEffect.velocityOverride = oppositeDirection;
		currentPullSpeedEffect = speedAlteringEffect;
		vitals.movement.OnAddDebuff (speedAlteringEffect);
	}
}
	public override void TerminateObjectFunctions () {
		base.TerminateObjectFunctions ();
		lineRenderer.enabled = false;
		isPulling = false;
		anchorTransform = null;
		cursorTransform = null;
	}
}
