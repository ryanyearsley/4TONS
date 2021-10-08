using UnityEngine;

public class RegenAllyMana : SpellEffect {
	[SerializeField]
	private float regenAmount;
	public override void OnAllyHit (VitalsEntity caster, VitalsEntity ally) {
		if (ally.resource != null) {
			ally.resource.RegenerateMana (regenAmount);
		}
	}
}