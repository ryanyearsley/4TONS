using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHookObject : ProjectileObject {
	[SerializeField]
	private float pullSpeed;
	[SerializeField]
	private float pullLocationThreshold = 0.5f;
	private SpeedAlteringEffect currentPullSpeedEffect;

	[SerializeField]
	private Transform targetPullLocationTransform;
	[SerializeField]
	private Transform projectileAnchorTransform;

	private Transform playerAnchorTransform;
	[SerializeField]
	private LineRenderer lineRenderer;

	private bool isPullingCaster = false;

	public override void SetupObject () {
		base.SetupObject ();
		targetPullLocationTransform = new GameObject("GrapplingHookTarget_" + GetInstanceID()).transform;
		targetPullLocationTransform.parent = trans;
		playerAnchorTransform = new GameObject ("GrapplingHookPlayer_" + GetInstanceID ()).transform;
		playerAnchorTransform.parent = trans;
		playerAnchorTransform.localPosition += Vector3.up * 0.325f;
	}
	public override void ReuseSpellObject (VitalsEntity casterVitals) {
		base.ReuseSpellObject (casterVitals);
		targetPullLocationTransform.parent = trans;
		targetPullLocationTransform.localPosition = Vector3.zero;
		playerAnchorTransform.parent = casterVitals.trans;
		playerAnchorTransform.localPosition = Vector3.up * 0.325f;
		lineRenderer.SetPosition (0, playerAnchorTransform.position);
		lineRenderer.SetPosition (1, projectileAnchorTransform.position);
		lineRenderer.enabled = true;
		isPullingCaster = false;

	}
	public override void Update () {
		base.Update ();
		if (isAlive) {
			lineRenderer.SetPosition (0, playerAnchorTransform.position);
			lineRenderer.SetPosition (1, projectileAnchorTransform.position);
			if (!isPullingCaster) {
				trans.Translate (Vector2.right * moveSpeed * Time.fixedDeltaTime);

			} else {
				trans.position = targetPullLocationTransform.position;
				UpdatePullVelocityOverride ();
			}
		}
	}

	public override void OnWallHit (Collider2D other) {
		if (!isPullingCaster) {
			targetPullLocationTransform.parent = null;
			targetPullLocationTransform.position = trans.position;
		}
			ApplyPullPlayerEffect ();
	}
	public override void OnEnemyHit (VitalsEntity enemyVitals) {
		if (!isPullingCaster) {
			foreach (SpellEffect spellEffect in spellEffects) {
				spellEffect.OnEnemyHit (casterVitalsEntity, enemyVitals);
			}
			targetPullLocationTransform.parent = enemyVitals.trans;
			targetPullLocationTransform.localPosition = Vector3.up * 0.325f;
			ApplyPullPlayerEffect ();
		}
	}

	public void ApplyPullPlayerEffect () {
		if (casterVitalsEntity.movement != null) {
			SpeedAlteringEffect speedAlteringEffect = new SpeedAlteringEffect (1, lifeTime, true);
			speedAlteringEffect.isVelocityOverride = true;
			currentPullSpeedEffect = speedAlteringEffect;
			casterVitalsEntity.movement.OnAddDebuff (speedAlteringEffect);
			UpdatePullVelocityOverride ();
		}
		isPullingCaster = true;
		lifeTimer = 0f;
	}

	private void UpdatePullVelocityOverride() {
		if (currentPullSpeedEffect != null) {

			Vector2 pullDirection = ((Vector2)targetPullLocationTransform.position - (Vector2)playerAnchorTransform.position).normalized;
			currentPullSpeedEffect.velocityOverride = pullDirection * pullSpeed;
			if (IsometricCoordinateUtilites.RawDistanceBetweenPoints (targetPullLocationTransform, casterVitalsEntity.trans) < pullLocationThreshold) {
				currentPullSpeedEffect.effectTimer = currentPullSpeedEffect.effectTime;
				Destroy ();
			}
		}
	}
	public override void TerminateObjectFunctions () {
		base.TerminateObjectFunctions ();
		lineRenderer.enabled = false;
		isPullingCaster = false;
		playerAnchorTransform.parent = trans;
		targetPullLocationTransform.parent = trans;
		currentPullSpeedEffect = null;
	}
}
