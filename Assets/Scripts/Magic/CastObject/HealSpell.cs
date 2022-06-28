using UnityEngine;

public class HealSpell : Spell
{
    public float healAmount;
    public float manaRegenAmount;

    public override void CastSpell () {
        base.CastSpell ();
        playerObject.vitalsEntity.health.Heal (healAmount);
        playerObject.vitalsEntity.resource.RegenerateMana (manaRegenAmount);
        if (spellData.spellObject != null) {
            PoolManager.instance.ReuseObject (spellData.spellObject, playerObject.vitalsEntity.creatureObject.transform.position, Quaternion.identity);
        }
    }
}
