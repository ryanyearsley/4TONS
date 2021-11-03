using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullEnemyToCaster : SpellEffect
{
	private Rigidbody2D rb;
	[SerializeField]
	private float pullSpeed = 3;
	[SerializeField]
	private float duration;

	public override void SetUpSpellEffect () {
		rb = GetComponentInParent<Rigidbody2D> ();
	}
	public override void OnEnemyHit (VitalsEntity caster, VitalsEntity vitals) {
		if (vitals.movement != null) {
			SpeedAlteringEffect speedAlteringEffect = new SpeedAlteringEffect (1, duration, false);
			speedAlteringEffect.isVelocityOverride = true;
			Vector2 oppositeDirection = transform.right * -pullSpeed;
			Debug.Log ("PullEnemyToCaster: OppositeDirection: " + oppositeDirection);
			speedAlteringEffect.velocityOverride = oppositeDirection;
			vitals.movement.OnAddDebuff (speedAlteringEffect);
		}
	}
}
