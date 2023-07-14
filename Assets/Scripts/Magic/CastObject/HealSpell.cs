using UnityEngine;

public class HealSpell : Spell
{
    public float healAmount;
    public float manaRegenAmount;

    public override void CastSpell () {
        base.CastSpell ();
        playerObject.vitalsEntity.health.Heal (healAmount);
        playerObject.vitalsEntity.resource.RegenerateMana (manaRegenAmount);
        if (debrisObject != null) {
            PoolManager.instance.ReuseObject (debrisObject, playerObject.vitalsEntity.creatureObject.transform.position, Quaternion.identity);
        }
    }
}
