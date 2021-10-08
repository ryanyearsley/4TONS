using UnityEngine;

public abstract class SpellEffect : MonoBehaviour
{
	public virtual void OnEnemyHit (VitalsEntity caster, VitalsEntity enemy) {

	}

	public virtual void OnAllyHit (VitalsEntity caster, VitalsEntity ally) {

	}

}
