using UnityEngine;

public class Snare : SpellEffect {

	[SerializeField]
	private float speedReduction;
	[SerializeField]
	private float duration;

	public override void OnEnemyHit (VitalsEntity caster, VitalsEntity vitals) {
		if (vitals.movement != null) {
			vitals.movement.OnAddDebuff (new SpeedAlteringEffect (speedReduction, duration, true));
			Debug.Log ("SpellEffect: Snaring enemy by " + speedReduction + " for " + duration + " seconds.");

		}
	}
}